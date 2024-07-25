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
    public async Task CreateDefaultInstallerAsync()
    {
        var email = "emmanuel@gmail.com";
        var fun = "Pa$$w0rd";
        _logger.LogInformation("Attempting to seed default installer");

        User? installerAccount = await _userManager.FindByEmailAsync(email);
        if (installerAccount is not null)
        {
            _logger.LogInformation("Account {Email} exists. Adding to {Role} role",
                email,
                ApplicationRole.Installer);
            await _accountService.AddAccountToRoleAsync(installerAccount.Id, ApplicationRole.Installer);
            return;
        }

        State? state = await _unitOfWork.States.FirstOrDefaultAsync();
        if (state is null)
        {
            _logger.LogError("There is no state in the database. Cannot proceed with seeding installer");
            return;
        }

        CreateInstallerRequest installer = new()
        {
            FirstName = "Emmanuel",
            LastName = "Allison",
            Email = email,
            Username = "hallixon",
            PhoneNumber = "",
            Password = fun,
            PasswordConfirmation = fun,
            StateId = state.Id,
            City = "Port Harcourt",
            CompanyName = "Port Harcourt Electrical Distribution Company",
            HouseNumber = "House 8",
            Street = "Ada George",
        };
        Result result = await _accountService.CreateInstallerAsync(installer);

        if (result.IsFailure)
        {
            _logger.LogWarning("Default {Role} account not seeded. Error: {@Error}",
                ApplicationRole.Installer,
                result.Error);

            return;
        }

        _logger.LogInformation("Default {Role} account seeded", ApplicationRole.Installer);
    }
}
