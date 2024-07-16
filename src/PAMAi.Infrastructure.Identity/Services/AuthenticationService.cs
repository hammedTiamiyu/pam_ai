using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;
using PAMAi.Infrastructure.Identity.Errors;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Services;

/// <inheritdoc cref="IAuthenticationService"/>
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(UserManager<User> userManager, ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> LoginAsync(
        LoginRequest credentials,
        ApplicationRole loginAsRole,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{Username} is attempting to log in as a {Role}", credentials.Username, loginAsRole);

        User? user = await _userManager.FindByEmailAsync(credentials.Username);
        // Try to find user by username if there was no email match.
        user ??= await _userManager.FindByNameAsync(credentials.Username);

        // If uer is null despite the search using their username and email.
        if (user is null)
        {
            _logger.LogError("Login failed. No account matches username/email '{Username}'", credentials.Username);

            return Result.Failure(AccountError.DoesNotExist);
        }

        // Check if user is in role.
        bool isInRole = await _userManager.IsInRoleAsync(user, loginAsRole.ToString());
        if (!isInRole)
        {
            _logger.LogError("Login failed. Account {Email} is not in {Role} role", credentials.Username, loginAsRole);
            Error error = AccountError.IsNotInRole with
            {
                Summary = string.Format(AccountError.IsNotInRole.Summary, loginAsRole),
            };

            return Result.Failure(error);
        }

        bool signedIn = await _userManager.CheckPasswordAsync(user, credentials.Password);

        if (!signedIn)
        {
            _logger.LogError("Login failed. {Email} attempted to log in with a wrong password", credentials.Username);

            return Result.Failure(AccountError.InvalidCredentials);
        }

        throw new NotImplementedException();
    }
}
