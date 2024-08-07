using System.Diagnostics;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;
using PAMAi.Domain.Enums;
using PAMAi.Infrastructure.Identity.Errors;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Services;

internal class AccountService: IAccountService
{
    private static readonly TypeAdapterConfig s_userToReadProfileResponseConfig = TypeAdapterConfig<User, ReadProfileResponse>
        .NewConfig()
        .Map(dest => dest.Username, src => src.UserName)
        .Config;

    private readonly UserManager<User> _userManager;
    private readonly IdentityContext _identityContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(
        UserManager<User> userManager,
        IdentityContext identityContext,
        ICurrentUser currentUser,
        ILogger<AccountService> logger,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _identityContext = identityContext;
        _currentUser = currentUser;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AddAccountToRoleAsync(string userId, ApplicationRole role)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            _logger.LogError("Could not add account {UserId} to role {Role}. Account does not exist",
                userId,
                role);

            return Result.Failure(AccountErrors.UnableToAddToRole with { Description = "Account does not exist." });
        }

        return await AddAccountToRoleAsync(user, role);
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest credentials, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Logged-in user is attempting to change their password");
        User? user = await _identityContext.Users
            .Where(u => u.Id == _currentUser.UserId!)
            .Include(u => u.UserPassword)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            _logger.LogError("Could not change password. Account {Id} can't be found", _currentUser.UserId);
            return Result.Failure(AccountErrors.UnableToChangePassword with
            {
                Description = "Account cannot be found.",
            });
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, credentials.OldPassword, credentials.NewPassword);
        if (!result.Succeeded)
        {
            IEnumerable<string> errors = result.Errors.Select(err => err.Description);
            _logger.LogError("Could not change password of user {Id}. Errors: {@Errors}", user.Id, errors);

            return Result.Failure(AccountErrors.UnableToChangePassword);
        }

        await UpsertUserPasswordAsync(user, credentials.NewPassword);
        _logger.LogInformation("User {Id} changed their password successfully", user.Id);

        return Result.Success();
    }

    public async Task<Result> CreateInstallerAsync(CreateInstallerRequest installer, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.FindByEmailAsync(installer.Email);
        if (user is not null)
        {
            _logger.LogError("An account exists for {Email}", installer.Email);

            return Result.Failure(AccountErrors.UnableToCreate with
            {
                Description = $"An account exists for {installer.Email}"
            });
        }

        user = new User
        {
            Email = installer.Email,
            UserName = installer.Username,
            PhoneNumber = installer.PhoneNumber,
        };
        UserProfile profile = installer.Adapt<UserProfile>();
        Result createUserResult = await CreateAccountAsync(user, profile, installer.Password, ApplicationRole.Installer, installer.AcceptTermsOfService);

        return createUserResult;
    }

    public async Task<Result> CreateSuperAdminAsync(CreateSuperAdminRequest superAdmin, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.FindByEmailAsync(superAdmin.Email);
        if (user is not null)
        {
            _logger.LogError("An account exists for {Email}", superAdmin.Email);

            return Result.Failure(AccountErrors.UnableToCreate with
            {
                Description = $"An account exists for {superAdmin.Email}"
            });
        }

        user = new User
        {
            Email = superAdmin.Email,
            UserName = superAdmin.Username,
            PhoneNumber = superAdmin.PhoneNumber,
        };
        UserProfile profile = superAdmin.Adapt<UserProfile>();
        Result createUserResult = await CreateAccountAsync(user, profile, superAdmin.Password, ApplicationRole.SuperAdmin, superAdmin.AcceptTermsOfService);

        return createUserResult;
    }

    public async Task<Result> DeleteAccountAsync(string accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            User? user = await _userManager.FindByIdAsync(accountId);
            UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(accountId);

            if (user is null)
            {
                _logger.LogInformation("Account {Id} does not exist", accountId);
                return Result.Success();
            }

            Result result = await DeleteAccountAsync(user);
            if (result.IsFailure)
            {
                return Result.Failure(AccountErrors.UnableToDelete);
            }

            if (userProfile is not null)
                _unitOfWork.UserProfiles.Remove(userProfile);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not delete account {Id}. Message: {Message}", accountId, exception.Message);
            return Result.Failure(AccountErrors.UnableToDelete);
        }
    }

    public async Task<Result<ReadProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("An attempt was made to get an account profile when there is no logged in user");
            return Result<ReadProfileResponse>.Failure(AccountErrors.Unauthorised);
        }

        return await GetProfileAsync(_currentUser.UserId!, cancellationToken);
    }

    public async Task<Result<ReadProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.FindByIdAsync(userId!);
        UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(userId!, cancellationToken);
        if (user is null)
        {
            _logger.LogError("User {Id} cannot be found in the database", userId);
            return Result<ReadProfileResponse>.Failure(AccountErrors.NotFound);
        }
        if (userProfile is null)
        {
            _logger.LogError("Profile for user {Id} cannot be found in the database", userId);
            return Result<ReadProfileResponse>.Failure(new Error("Internal server error") with
            {
                Description = "Could not find user profile.",
            });
        }

        ReadProfileResponse response = userProfile.Adapt<ReadProfileResponse>(ReadProfileResponse.FromUserProfile);
        user.Adapt(response, s_userToReadProfileResponseConfig);
        response.AcceptedTermsOfService = await _unitOfWork.UserTermsOfServiceConsents.IsCurrentTermsOfServiceAcceptedAsync(userProfile.Id, cancellationToken);

        return Result<ReadProfileResponse>.Success(response);
    }

    public async Task<Result<ReadProfileResponse>> UpdateProfileAsync(UpdateProfileRequest profile, CancellationToken cancellationToken = default)
    {
        UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(_currentUser.UserId!, cancellationToken);
        if (userProfile is null)
        {
            _logger.LogError("Profile for user {Id} cannot be found", _currentUser.UserId);
            return Result<ReadProfileResponse>.Failure(AccountErrors.UnableToUpdateProfile with
            {
                Description = "Could not find user profile.",
            });
        }

        if (profile.StateId is not null)
        {
            State? state = await _unitOfWork.States.FindAsync((long)profile.StateId, cancellationToken);
            if (state is null)
            {
                _logger.LogError("Cannot update profile. State {Id} does not exist", profile.StateId);
                return Result<ReadProfileResponse>.Failure(AccountErrors.UnableToUpdateProfile with
                {
                    Description = "State does not exist.",
                });
            }

            userProfile.State = state;
        }

        profile.Adapt(userProfile, UpdateProfileRequest.ToUserProfile);
        await _unitOfWork.CompleteAsync(cancellationToken);
        _logger.LogInformation("User {Id} updated their profile successfully", _currentUser.UserId);

        return await GetProfileAsync(cancellationToken);
    }

    async Task<Result<Guid>> IAccountService.CreateUserAsync(CreateUserRequest user)
    {
        Guid? profileId = await GetProfileIdAsync(user.Email);
        if (profileId is not null)
        {
            _logger.LogError("An account exists for {Email}", user.Email);

            return Result<Guid>.Success((Guid)profileId);
        }

        User userAccount = new()
        {
            Email = user.Email,
            UserName = user.Username,
            PhoneNumber = user.PhoneNumber,
        };
        UserProfile profile = user.Adapt<UserProfile>();
        Result createUserResult = await CreateAccountAsync(userAccount, profile, user.Password, ApplicationRole.User, false);

        if (createUserResult.IsFailure)
            return Result<Guid>.Failure(AccountErrors.UnableToCreate with
            {
                Description = "",
                InnerError = createUserResult.Error,
            });

        return Result<Guid>.Success(profile.Id);
    }

    async Task<Result> IAccountService.DeleteAccountAsync(Guid accountProfileId, CancellationToken cancellationToken)
    {
        try
        {
            UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(accountProfileId);
            if (userProfile is null)
            {
                _logger.LogError("Could not delete user. User profile {Id} is null", accountProfileId);
                return Result.Failure(AccountErrors.UnableToDelete with
                {
                    Description = $"User profile {accountProfileId.ToString()} cannot be found.",
                });
            }

            User? user = await _userManager.FindByIdAsync(userProfile.UserId.ToString());
            if (user is null)
            {
                _logger.LogInformation("Account {Id} does not exist", userProfile.UserId.ToString());
                return Result.Success();
            }

            Result result = await DeleteAccountAsync(user);
            if (result.IsFailure)
            {
                return Result.Failure(AccountErrors.UnableToDelete);
            }

            if (userProfile is not null)
                _unitOfWork.UserProfiles.Remove(userProfile);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not delete account {Id}. Message: {Message}", accountProfileId, exception.Message);
            return Result.Failure(AccountErrors.UnableToDelete);
        }
    }

    async Task<List<string>> IAccountService.GetSimilarUsernamesAsync(string username, CancellationToken cancellationToken)
    {
        var watch = Stopwatch.StartNew();
        List<string> similarUsernames = await _identityContext.Users
            .Where(u => EF.Functions.Like(u.UserName, $"%{username}%"))
            .Select(u => u.UserName ?? string.Empty)
            .ToListAsync(cancellationToken);
        watch.Stop();

        _logger.LogDebug("Fetched usernames similar to {Username} in {Time} ms. Matches: {Count}",
            username,
            watch.ElapsedMilliseconds,
            similarUsernames.Count);
        _logger.LogTrace("Similar usernames: {@MatchingUsernames}", similarUsernames);

        return similarUsernames;
    }

    async Task<string?> IAccountService.GetIdAsync(string email)
    {
        return await GetAccountIdAsync(email);
    }

    async Task<Guid?> IAccountService.GetProfileIdAsync(string email)
    {
        return await GetProfileIdAsync(email);
    }

    async Task<UserCredentials?> IAccountService.GetUserCredentialsAsync(string userId, CancellationToken cancellationToken)
    {
        User? user = await _identityContext.Users
            .Where(u => u.Id == userId)
            .Include(u => u.UserPassword)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            _logger.LogError("Could not get user credentials. User {Id} does not exist", userId);
            return null;
        }
        if (user.UserPassword is null)
        {
            _logger.LogError("Could not get user credentials. Credential of user {Id} not recorded", userId);
            return null;
        }

        _logger.LogTrace("Retrieved user credentials");

        return new UserCredentials
        {
            Password = Cryptography.Decrypt(user.UserPassword.Password),
            Email = user.Email!,
        };
    }

    private async Task<Result> AddAccountToRoleAsync(User user, ApplicationRole role)
    {
        _logger.LogInformation("Adding account {Email} to {Role} role", user.Email, role);

        byte triesAvailable = 4;

        bool isInRole = await _userManager.IsInRoleAsync(user, role.ToString());
        if (isInRole)
        {
            _logger.LogInformation("Account {Email} is already in {Role} role", user.Email, role);
            return Result.Success();
        }

        IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(
                user,
                role.ToString());

        while (!addToRoleResult.Succeeded && triesAvailable > 0)
        {
            _logger.LogWarning(
                "Adding account {Email} to role {Role} failed. Retrying operation. Tries available: {Tries}",
                role,
                user.Email,
                triesAvailable);

            addToRoleResult = await _userManager.AddToRoleAsync(
                user,
                role.ToString());
            triesAvailable--;
        }

        if (!addToRoleResult.Succeeded)
        {
            IEnumerable<string> errors = addToRoleResult.Errors.Select(err => err.Description);

            _logger.LogError("Could not add account {Email} to {Role} role. Errors: {@Errors}",
                user.Email,
                ApplicationRole.SuperAdmin,
                errors);

            return Result.Failure(AccountErrors.UnableToAddToRole);
        }

        _logger.LogInformation("Account {Email} added to {Role} role", user.Email, role);
        return Result.Success();
    }

    private async Task AddDefaultUserConsentAsync(Guid userProfileId, bool acceptCurrent = false)
    {
        var allTerms = await _unitOfWork.TermsOfService.GetAsync();
        foreach (var termsOfService in allTerms)
        {
            UserTermsOfServiceConsent userTermsOfServiceConsent = new()
            {
                TermsOfServiceId = termsOfService.Id,
                UserProfileId = userProfileId,
                // Accept this ToS if it is the active one, and the user indicated their acceptance.
                AcceptedDateUtc = (acceptCurrent && termsOfService.IsActive) ? DateTimeOffset.UtcNow : null,
            };
            await _unitOfWork.UserTermsOfServiceConsents.AddAsync(userTermsOfServiceConsent);
        }
        await _unitOfWork.CompleteAsync();
    }

    private async Task<Result> CreateAccountAsync(User user, UserProfile profile, string password, ApplicationRole role, bool acceptTermsOfService)
    {
        _logger.LogInformation("Adding account {Email}", user.Email);

        int triesAvailable = 4;
        IdentityResult result = await _userManager.CreateAsync(user, password);

        while (!result.Succeeded && triesAvailable > 0)
        {
            _logger.LogWarning(
                "Add account {Email} failed. Retrying operation. Tries available: {Tries}",
                user.Email,
                triesAvailable);
            result = await _userManager.CreateAsync(user, password);
            triesAvailable--;
        }

        if (!result.Succeeded)
        {
            IEnumerable<string> errors = result.Errors.Select(err => err.Description);
            _logger.LogError("Could not add account {Email} to {Role} role. Errors: {@Errors}",
                user.Email,
                ApplicationRole.SuperAdmin,
                errors);

            return Result.Failure(AccountErrors.UnableToCreate);
        }

        user.UserPassword = new UserPassword
        {
            Password = Cryptography.Encrypt(password),
        };
        _identityContext.Users.Update(user);
        await _identityContext.SaveChangesAsync();

        Result addToRoleResult = await AddAccountToRoleAsync(user, role);
        if (addToRoleResult.IsFailure)
        {
            await DeleteAccountAsync(user);
            return Result.Failure(AccountErrors.UnableToCreate with { InnerError = addToRoleResult.Error });
        }

        try
        {
            profile.UserId = Guid.Parse(user.Id);
            await _unitOfWork.UserProfiles.AddAsync(profile);
            await _unitOfWork.CompleteAsync();
            await AddDefaultUserConsentAsync(profile.Id, acceptTermsOfService);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred. Message: {Message}", exception.Message);
            await DeleteAccountAsync(user);

            return Result.Failure(AccountErrors.UnableToCreate);
        }

        _logger.LogInformation("Account {Email} added", user.Email);
        return Result.Success();
    }

    private async Task<Result> DeleteAccountAsync(User user)
    {
        _logger.LogInformation("Deleting account {Email}", user.Email);

        string email = user.Email!;
        IdentityResult operationResult = await _userManager.DeleteAsync(user);
        if (!operationResult.Succeeded)
        {
            IEnumerable<string> errors = operationResult.Errors.Select(err => err.Description);
            _logger.LogInformation("Could not delete account {Email}. Errors: {@Errors}",
                email,
                errors);

            return Result.Failure(AccountErrors.UnableToDelete);
        }

        _logger.LogInformation("Account {Email} deleted", email);
        return Result.Success();
    }

    private async Task<string?> GetAccountIdAsync(string email)
    {
        User? user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
            _logger.LogDebug("Found matching account for {Email}. Account Id: {Id}", email, user.Id);
        else
            _logger.LogDebug("Matching account for {Email} not found", email);

        return user?.Id;
    }

    private async Task<Guid?> GetProfileIdAsync(string email)
    {
        string? userId = await GetAccountIdAsync(email);
        if (userId is null)
            return null;

        UserProfile? profile = await _unitOfWork.UserProfiles.FindAsync(userId);
        if (profile is not null)
            _logger.LogDebug("Found matching profile for {Email}. Profile Id: {Id}", email, profile.Id);
        else
            _logger.LogDebug("Matching profile for {Email} not found", email);

        return profile?.Id;
    }

    /// <summary>
    /// Insert a <see cref="UserPassword"/> record for the existing user
    /// if they don't currently have one.
    /// </summary>
    /// <param name="user">
    /// User account.
    /// </param>
    /// <param name="password">
    /// User password.
    /// </param>
    /// <returns></returns>
    private async Task UpsertUserPasswordAsync(User user, string password)
    {
        if (user.UserPassword is not null)
        {
            _logger.LogTrace("{Name} exists for user {Id}", nameof(UserPassword), user.Id);
            user.UserPassword.Password = Cryptography.Encrypt(password);
        }
        else
        {
            user.UserPassword = new UserPassword
            {
                Password = Cryptography.Encrypt(password),
            };
        }

        _identityContext.Users.Update(user);
        await _identityContext.SaveChangesAsync();
        _logger.LogTrace("{Name} upserted for user {Id}", nameof(UserPassword), user.Id);
    }
}
