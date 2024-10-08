﻿using Microsoft.AspNetCore.Http;
using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class AccountErrors
{
    internal static readonly Error NotFound = new("Not found", StatusCodes.Status404NotFound);
    internal static readonly Error UnableToChangePassword = new("Unable to change password");
    internal static readonly Error UnableToCreate = new("Unable to create account");
    internal static readonly Error UnableToAddToRole = new("Unable to add user to role");
    internal static readonly Error UnableToDelete = new("Unable to delete user");
    internal static readonly Error UnableToUpdateProfile = new("Unable to update user profile");
    internal static readonly Error Unauthorised = new("Unauthorised", StatusCodes.Status401Unauthorized);
}
