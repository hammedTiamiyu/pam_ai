using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Services.Interfaces;
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
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        ILogger<AccountService> logger,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
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

            return Result.Failure(AccountError.UnableToAddToRole with { Description = "Account does not exist." });
        }

        return await AddAccountToRoleAsync(user, role);
    }

    public async Task<Result> CreateInstallerAsync(CreateInstallerRequest installer, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.FindByEmailAsync(installer.Email);
        if (user is not null)
        {
            _logger.LogError("An account exists for {Email}", installer.Email);

            return Result.Failure(AccountError.UnableToCreate with
            {
                Description = $"An account exists for {installer.Email}"
            });
        }

        user = new User
        {
            FirstName = installer.FirstName,
            LastName = installer.LastName,
            Email = installer.Email,
            UserName = installer.Username,
            PhoneNumber = installer.PhoneNumber,
        };
        UserProfile profile = installer.Adapt<UserProfile>();
        Result createUserResult = await CreateAccountAsync(user, profile, installer.Password, ApplicationRole.Installer);

        return createUserResult;
    }

    public async Task<Result> CreateSuperAdminAsync(CreateSuperAdminRequest superAdmin, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.FindByEmailAsync(superAdmin.Email);
        if (user is not null)
        {
            _logger.LogError("An account exists for {Email}", superAdmin.Email);

            return Result.Failure(AccountError.UnableToCreate with
            {
                Description = $"An account exists for {superAdmin.Email}"
            });
        }

        user = new User
        {
            FirstName = superAdmin.FirstName,
            LastName = superAdmin.LastName,
            Email = superAdmin.Email,
            UserName = superAdmin.Username,
            PhoneNumber = superAdmin.PhoneNumber,
        };
        UserProfile profile = new()
        {
            StateId = superAdmin.StateId,
        };

        Result createUserResult = await CreateAccountAsync(user, profile, superAdmin.Password, ApplicationRole.SuperAdmin);

        return createUserResult;
    }

    public Task<Result> CreateUserAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public async Task<Result<ReadProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("An attempt was made to get an account profile when there is no logged in user");
            return Result<ReadProfileResponse>.Failure(AccountError.Unauthorised);
        }

        User? user = await _userManager.FindByIdAsync(_currentUser.UserId!);
        UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(_currentUser.UserId!, cancellationToken);
        if (user is null)
        {
            _logger.LogError("User {Id} cannot be found in the database", _currentUser.UserId);
            return Result<ReadProfileResponse>.Failure(AccountError.Unauthorised);
        }
        if (userProfile is null)
        {
            _logger.LogError("Profile for user {Id} cannot be found in the database", _currentUser.UserId);
            return Result<ReadProfileResponse>.Failure(new Error("Internal server error") with
            {
                Description = "Could not find user profile.",
            });
        }

        ReadProfileResponse response = userProfile.Adapt<ReadProfileResponse>(ReadProfileResponse.FromUserProfile);
        user.Adapt(response, s_userToReadProfileResponseConfig);

        return Result<ReadProfileResponse>.Success(response);
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

            return Result.Failure(AccountError.UnableToAddToRole);
        }

        _logger.LogInformation("Account {Email} added to {Role} role", user.Email, role);
        return Result.Success();
    }

    private async Task<Result> CreateAccountAsync(User user, UserProfile profile, string password, ApplicationRole role)
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

            return Result.Failure(AccountError.UnableToCreate);
        }

        Result addToRoleResult = await AddAccountToRoleAsync(user, role);
        if (addToRoleResult.IsFailure)
        {
            await DeleteAccountAsync(user);
            return Result.Failure(AccountError.UnableToCreate with { InnerError = addToRoleResult.Error });
        }

        try
        {
            profile.UserId = Guid.Parse(user.Id);
            await _unitOfWork.UserProfiles.AddAsync(profile);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred. Message: {Message}", exception.Message);
            await DeleteAccountAsync(user);

            return Result.Failure(AccountError.UnableToCreate);
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

            return Result.Failure(AccountError.UnableToDelete);
        }

        _logger.LogInformation("Account {Email} deleted", email);
        return Result.Success();
    }
}
