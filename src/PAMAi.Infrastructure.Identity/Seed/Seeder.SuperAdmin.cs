using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Dto.Account;
using PAMAi.Domain.Entities;
using PAMAi.Domain.Enums;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Seed;

internal sealed partial class Seeder
{
    /// <summary>
    /// Creates the application's default super admin user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> to monitor the operation.
    /// </returns>
    public async Task CreateDefaultSuperAdminAsync()
    {
        var email = "sbo@example.com";
        var password = "P@55w0Rd";
        _logger.LogInformation("Attempting to seed default super admin");

        User? superAdminUser = await _userManager.FindByEmailAsync(email);
        if (superAdminUser is not null)
        {
            _logger.LogInformation("Account {Email} exists. Adding to {Role} role",
                email,
                ApplicationRole.SuperAdmin);
            await _accountService.AddAccountToRoleAsync(superAdminUser.Id, ApplicationRole.SuperAdmin);
            return;
        }

        State? state = await _unitOfWork.States.FirstOrDefaultAsync();
        if (state is null)
        {
            _logger.LogError("There is no state in the database. Cannot proceed with seeding super admin");
            return;
        }

        CreateSuperAdminRequest superAdmin = new()
        {
            FirstName = "Babatunde",
            LastName = "Ogundele",
            Email = email,
            Username = "SBO",
            PhoneNumber = "",
            Password = password,
            PasswordConfirmation = password,
            StateId = state.Id,
        };
        Result result = await _accountService.CreateSuperAdminAsync(superAdmin);

        if (result.IsFailure)
        {
            _logger.LogWarning("Default {Role} account not seeded. Error: {@Error}",
                ApplicationRole.SuperAdmin,
                result.Error);

            return;
        }

        _logger.LogInformation("Default {Role} account seeded", ApplicationRole.SuperAdmin);
    }
}
