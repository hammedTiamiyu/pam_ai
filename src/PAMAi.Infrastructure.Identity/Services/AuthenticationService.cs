using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Infrastructure.Identity.Errors;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Services;
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(UserManager<User> userManager, ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> LoginAsync(LoginRequest credentials, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{Username} is attempting to log in", credentials.Username);

        User? user = await _userManager.FindByEmailAsync(credentials.Username);
        // Try to find user by username if there was no email match.
        user ??= await _userManager.FindByNameAsync(credentials.Username);

        // If uer is null despite the search using their username and email.
        if (user is null)
        {
            _logger.LogInformation("No user account matches username '{Username}'", credentials.Username);

            return Result.Failure(AccountError.DoesNotExist);
        }

        throw new NotImplementedException();
    }
}
