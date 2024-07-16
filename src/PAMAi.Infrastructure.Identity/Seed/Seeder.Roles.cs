using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PAMAi.Domain.Enums;

namespace PAMAi.Infrastructure.Identity.Seed;

/// <summary>
/// Performs database seeding for the identity layer.
/// </summary>
internal sealed partial class Seeder
{
    /// <summary>
    /// Creates the application's default roles in the database.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> to monitor the operation.
    /// </returns>
    public async Task CreateApplicationRolesAsync()
    {
        _logger.LogInformation("Attempting to seed application roles");

        foreach (ApplicationRole role in Enum.GetValues<ApplicationRole>())
        {
            var roleExists = await _roleManager.RoleExistsAsync(role.ToString());

            if (roleExists)
            {
                _logger.LogInformation("{Role} role exists. Skipping role creation", role);
                continue;
            }

            IdentityRole identityRole = new(role.ToString());
            IdentityResult result = await _roleManager.CreateAsync(identityRole);

            if (!result.Succeeded)
            {
                List<string> errors = result.Errors
                    .Select(err => err.Description)
                    .ToList();
                _logger.LogWarning("{Role} role was not created successfully. Errors: {@Errors}",
                    role,
                    errors);
                continue;
            }

            _logger.LogInformation("{Role} role created", role);
        }
    }
}
