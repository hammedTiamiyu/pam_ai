using Microsoft.AspNetCore.Http;
using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class AccountError
{
    internal static readonly Error DoesNotExist = new("Account.DoesNotExist", "User account does not exist", StatusCodes.Status404NotFound);
    internal static readonly Error Exists = new("Account.Exists", "User account does not exist", StatusCodes.Status409Conflict);
    internal static readonly Error UnableToCreate = new("Account.UnableToCreate", "Could not create account");
    internal static readonly Error UnableToAddToRole = new("Account.UnableToAddToRole", "Could not add user to role");
    internal static readonly Error UnableToDelete = new("Account.UnableToDelete", "Unable to delete user");
    internal static readonly Error IsNotInRole = new("Account.IsNotInRole", "User is not in {0} role");
    internal static readonly Error InvalidCredentials = new("Account.InvalidCredentials", "Invalid credentials");
}
