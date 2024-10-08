﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PAMAi.Application;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Extensions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;
using PAMAi.Domain.Enums;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.Identity.Errors;
using PAMAi.Infrastructure.Identity.Models;
using PAMAi.Infrastructure.Identity.Resources;
using IAuthenticationService = PAMAi.Application.Services.Interfaces.IAuthenticationService;

namespace PAMAi.Infrastructure.Identity.Services;

/// <inheritdoc cref="IAuthenticationService"/>
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly INotificationService _notificationService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityContext _identityContext;
    private readonly JwtOptions _jwtOptions;

    public AuthenticationService(
        UserManager<User> userManager,
        ILogger<AuthenticationService> logger,
        INotificationService notificationService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IdentityContext identityContext,
        IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot)
    {
        _userManager = userManager;
        _logger = logger;
        _notificationService = notificationService;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _identityContext = identityContext;
        _jwtOptions = jwtOptionsSnapshot.Value;
    }

    public async Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        _logger.LogInformation("{Email} initiated a forgot-password request", request.Email);

        // Generic response to send to the user regardless of the result.
        // See: https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html.
        ForgotPasswordResponse response = new()
        {
            Message = $"Email sent to {request.Email}.",
        };

        User? user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogError("Forgot password failed. No account matches {Email}", request.Email);
            return Result<ForgotPasswordResponse>.Success(response);
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var queryParameters = new Dictionary<string, string?>
        {
            { "email", request.Email },
            { "token", token },
        };
        string callbackUrl = QueryHelpers.AddQueryString(request.CallbackUrl, queryParameters);
        await SendResetPasswordLinkAsync(callbackUrl, user.Id);
        _logger.LogInformation("Reset password link sent to {Email}", request.Email);

        return Result<ForgotPasswordResponse>.Success(response);
    }

    [Obsolete]
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest credentials, CancellationToken cancellationToken = default)
    {
        // HACK: This was done at the persistent request of the FE devs for a quick demo. Should be removed.

        _logger.LogInformation("{Username} is attempting to login", credentials.Username);

        User? user = await _identityContext.Users
            .Where(FilterByUniqueId(credentials.Username))
            .Include(u => u.UserPassword)
            .FirstOrDefaultAsync(cancellationToken);

        // If user is null despite the search using their username and email.
        if (user is null)
        {
            _logger.LogError("Login failed. No account matches username/email '{Username}'", credentials.Username);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = $"No account exists for '{credentials.Username}'.",
            });
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        _logger.LogDebug("{User}'s roles: {@Roles}", credentials.Username, roles);
        if (roles.Count < 1)
        {
            _logger.LogError("Login failed. Account is not assigned to any roles and cannot be automatically signed in");
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = "Account has no roles.",
            });
        }

        string roleString = roles.First();
        _logger.LogInformation("Attempting to log in {User} using the role '{Role}'", credentials.Username, roleString);
        bool parseWorked = Enum.TryParse(roleString, true, out ApplicationRole role);
        if (!parseWorked)
        {
            _logger.LogError("Login failed. Account's role '{Role}' is not defined", roleString);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = "Account's role is not defined.",
            });
        }

        return await LoginAsync(user, role, credentials.Password, cancellationToken);
    }

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest credentials,
        ApplicationRole loginAsRole,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{Username} is attempting to log in as a {Role}", credentials.Username, loginAsRole);

        User? user = await _identityContext.Users
            .Where(FilterByUniqueId(credentials.Username))
            .Include(u => u.UserPassword)
            .FirstOrDefaultAsync(cancellationToken);

        // If user is null despite the search using their username and email.
        if (user is null)
        {
            _logger.LogError("Login failed. No account matches username/email '{Username}'", credentials.Username);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = $"No account exists for '{credentials.Username}'.",
            });
        }

        // Check if user is in role.
        bool isInRole = await _userManager.IsInRoleAsync(user, loginAsRole.ToString());
        if (!isInRole)
        {
            _logger.LogError("Login failed. Account '{Username}' is not in {Role} role", credentials.Username, loginAsRole);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = $"Account is not in {loginAsRole} role.",
            });
        }

        //bool loginSuccessful = await _userManager.CheckPasswordAsync(user, credentials.Password);
        //if (!loginSuccessful)
        //{
        //    _logger.LogError("Login failed. '{Username}' attempted to log in with a wrong password", credentials.Username);
        //    return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
        //    {
        //        Description = "Invalid credentials.",
        //    });
        //}

        //await InsertUserPasswordIfNotExistAsync(user, credentials.Password);
        //ClaimsIdentity claimsIdentity = await GetUserClaimsIdentityAsync(user, loginAsRole);
        //LoginResponse response = _tokenService.GenerateToken(claimsIdentity).Adapt<LoginResponse>();
        //var createdUtc = DateTimeOffset.UtcNow;

        //user.RefreshTokens.Add(new()
        //{
        //    Token = response.RefreshToken.ToSha512Base64UrlEncoding(),
        //    CreatedUtc = createdUtc,
        //    ExpiresUtc = createdUtc.AddDays(_jwtOptions.RefreshTokenValidForInDays),
        //});

        //_identityContext.Update(user);
        //await _identityContext.SaveChangesAsync(cancellationToken);
        //_logger.LogInformation("'{Username}' logged in successfully", credentials.Username);

        //return Result<LoginResponse>.Success(response);
        return await LoginAsync(user, loginAsRole, credentials.Password, cancellationToken);
    }

    public async Task<Result> LogoutAsync(string accessToken, string refreshToken)
    {
        // First, ensure the JWT was created and signed by this application.
        bool hasValidSignature = await _tokenService.ValidateTokenSignatureAsync(accessToken);
        if (!hasValidSignature)
        {
            _logger.LogWarning("An attempt to logout was made with an invalid access token. Info: {@info}",
                new
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                });
            return Result.Failure(AuthenticationErrors.LogoutFailed with
            {
                Description = "Invalid access token.",
            });
        }

        User? user = await GetUserByRefreshTokenAsync(refreshToken);
        if (user is null)
        {
            _logger.LogInformation("Logout failed. Refresh token {Token} does not match any user", refreshToken);

            return Result.Failure(AuthenticationErrors.LogoutFailed with
            {
                Description = "Invalid refresh token.",
            });
        }

        user = MarkRefreshTokenAsRevoked(user, refreshToken);
        await _tokenService.BlacklistJwtAsync(accessToken);
        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync();
        _logger.LogInformation("User {Email} logged out", user.Email);

        return Result.Success();
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest tokens, CancellationToken cancellationToken = default)
    {
        // First, ensure the JWT was created and signed by this application.
        bool hasValidSignature = await _tokenService.ValidateTokenSignatureAsync(tokens.AccessToken, cancellationToken);
        if (!hasValidSignature)
        {
            _logger.LogWarning("An attempt to logout was made with an invalid access token. Info: {@info}",
                new
                {
                    Token = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                });
            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.RefreshTokenFailed with
            {
                Description = "Invalid access token.",
            });
        }

        User? user = await GetUserByRefreshTokenAsync(tokens.RefreshToken);
        if (user is null)
        {
            _logger.LogInformation("Logout failed. Refresh token {Token} does not match any user", tokens.RefreshToken);

            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.RefreshTokenFailed with
            {
                Description = "Invalid refresh token.",
            });
        }

        RefreshToken refreshToken = user.RefreshTokens
            .First(r => r.Token == tokens.RefreshToken.ToSha512Base64UrlEncoding());

        ApplicationRole role = GetRoleFromJwt(tokens.AccessToken);
        ClaimsIdentity claimsIdentity = await GetUserClaimsIdentityAsync(user, role);
        RefreshTokenResponse response = _tokenService.GenerateToken(claimsIdentity).Adapt<RefreshTokenResponse>();
        var createdUtc = DateTimeOffset.UtcNow;

        user.RefreshTokens.Remove(refreshToken);
        user.RefreshTokens.Add(new()
        {
            Token = response.RefreshToken.ToSha512Base64UrlEncoding(),
            CreatedUtc = createdUtc,
            ExpiresUtc = createdUtc.AddDays(_jwtOptions.RefreshTokenValidForInDays),
        });

        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("'{Username}' refreshed their access token successfully", user.Email);

        return Result<RefreshTokenResponse>.Success(response);
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        _logger.LogInformation("{Email} is attempting to reset their password", request.Email);
        User? user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogError("Reset password failed. No account matches {Email}", request.Email);
            return Result.Failure(AuthenticationErrors.ResetPasswordFailed with
            {
                Description = $"No account matches {request.Email}.",
                StatusCode = StatusCodes.Status403Forbidden,
            });
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            _logger.LogError("Reset password failed. Identity errors: {@Errors}", errors);

            return Result.Failure(AuthenticationErrors.ResetPasswordFailed);
        }

        _logger.LogInformation("Account {Email} has reset their password", request.Email);
        return Result.Success();
    }

    private static User MarkRefreshTokenAsRevoked(User user, string refreshToken)
    {
        string hashed = refreshToken.ToSha512Base64UrlEncoding();

        user.RefreshTokens
            .First(r => r.Token.Equals(hashed, StringComparison.OrdinalIgnoreCase))
            .RevokedUtc = DateTimeOffset.UtcNow;

        return user;
    }

    /// <summary>
    /// Filter users by their email, username or phone number.
    /// </summary>
    /// <param name="uniqueField">
    /// Email, username or phone number.
    /// </param>
    /// <returns></returns>
    private static Expression<Func<User, bool>> FilterByUniqueId(string uniqueField)
    {
        return user =>
            user.Email == uniqueField ||
            user.UserName == uniqueField ||
            user.PhoneNumber == uniqueField;
    }

    private ApplicationRole GetRoleFromJwt(string jwt)
    {
        JwtSecurityToken securityToken = _tokenService.GetJwtSecurityToken(jwt);
        object role = securityToken.Payload
            .First(claim => string.Equals(claim.Key, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Value;
        _logger.LogTrace("Current role as seen in JWT: {Role}", role);

        return Enum.Parse<ApplicationRole>(role?.ToString() ?? string.Empty);
    }

    private async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        string hashed = refreshToken.ToSha512Base64UrlEncoding();

        User? user = await _identityContext.Users
            .Where(u => u.RefreshTokens.Any(r => EF.Functions.Like(r.Token, hashed)))
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync();

        _logger.LogTrace("Refresh token {Token} does not match any user", refreshToken);

        return user;
    }

    private async Task<ClaimsIdentity> GetUserClaimsIdentityAsync(User user, ApplicationRole role)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IEnumerable<Claim> claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Role, role.ToString()),
        }.Union(userClaims);

        return new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
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
    private async Task InsertUserPasswordIfNotExistAsync(User user, string password)
    {
        if (user.UserPassword is not null)
        {
            _logger.LogTrace("{Name} exists for user {Id}", nameof(UserPassword), user.Id);
            return;
        }

        user.UserPassword = new UserPassword
        {
            Password = Cryptography.Encrypt(password),
        };
        _identityContext.Users.Update(user);
        await _identityContext.SaveChangesAsync();
        _logger.LogTrace("{Name} inserted for user {Id}", nameof(UserPassword), user.Id);
    }

    private async Task<Result<LoginResponse>> LoginAsync(User user, ApplicationRole role, string password, CancellationToken cancellationToken)
    {
        bool loginSuccessful = await _userManager.CheckPasswordAsync(user, password);
        if (!loginSuccessful)
        {
            _logger.LogError("Login failed. '{Username}' attempted to log in with a wrong password", user.UserName);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = "Invalid credentials.",
            });
        }

        await InsertUserPasswordIfNotExistAsync(user, password);
        ClaimsIdentity claimsIdentity = await GetUserClaimsIdentityAsync(user, role);
        LoginResponse response = _tokenService.GenerateToken(claimsIdentity).Adapt<LoginResponse>();
        UserProfile? userProfile = await _unitOfWork.UserProfiles.FindAsync(user.Id, cancellationToken);
        response.HasAcceptedTermsOfService = await _unitOfWork.UserTermsOfServiceConsents.IsCurrentTermsOfServiceAcceptedAsync(
            userProfile!.Id,
            cancellationToken);
        response.Role = role;
        var createdUtc = DateTimeOffset.UtcNow;

        user.RefreshTokens.Add(new()
        {
            Token = response.RefreshToken.ToSha512Base64UrlEncoding(),
            CreatedUtc = createdUtc,
            ExpiresUtc = createdUtc.AddDays(_jwtOptions.RefreshTokenValidForInDays),
        });

        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("'{Username}' logged in successfully", user.UserName);

        return Result<LoginResponse>.Success(response);
    }

    private async Task SendResetPasswordLinkAsync(string link, string recipientUserId)
    {
        var messageBody = string.Format(SmsMessages.ResetPassword, link);
        var emailBody = string.Format(EmailMessages.ResetPassword, link);

        NotificationContents contents = new()
        {
            RecipientUserId = recipientUserId,
            Sms = new()
            {
                Message = messageBody
            },
            Email = new()
            {
                Body = emailBody,
                Format = EmailBodyFormat.Text,
                Subject = "Reset Password",
            }
        };

        await _notificationService.SendAsync(contents, NotificationChannels.Sms | NotificationChannels.Email);
    }
}
