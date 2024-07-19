﻿using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class AccountError
{
    internal static readonly Error UnableToCreate = new("Unable to create account");
    internal static readonly Error UnableToAddToRole = new("Unable to add user to role");
    internal static readonly Error UnableToDelete = new("Unable to delete user");
}
