﻿using PAMAi.Application.Dto.Account;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for account operations.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Adds an existing account to a role.
    /// </summary>
    /// <param name="userId">
    /// Account's user ID.
    /// </param>
    /// <param name="role">
    /// New role.
    /// </param>
    /// <returns></returns>
    Task<Result> AddAccountToRoleAsync(string userId, ApplicationRole role);

    /// <summary>
    /// Change password of the current logged-in user.
    /// </summary>
    /// <param name="credentials">
    /// New and old passwords.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> ChangePasswordAsync(ChangePasswordRequest credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create installer account.
    /// </summary>
    /// <param name="installer">
    /// Installer or engineer information.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the request.
    /// </param>
    /// <returns></returns>
    Task<Result> CreateInstallerAsync(CreateInstallerRequest installer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create super admin account.
    /// add the account the to super admin role.
    /// </summary>
    /// <param name="superAdmin">
    /// Super administrator's information.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the request.
    /// </param>
    /// <returns></returns>
    Task<Result> CreateSuperAdminAsync(CreateSuperAdminRequest superAdmin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete account.
    /// </summary>
    /// <param name="accountId">
    /// Account ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns></returns>
    Task<Result> DeleteAccountAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user's profile information.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// The user's profile information.
    /// </returns>
    Task<Result<ReadProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user's profile information.
    /// </summary>
    /// <param name="userId">User's ID.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>The user's profile information.</returns>
    Task<Result<ReadProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update logged-in user's profile.
    /// </summary>
    /// <param name="profile">
    /// Updated profile.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The updated user's profile information.
    /// </returns>
    Task<Result<ReadProfileResponse>> UpdateProfileAsync(UpdateProfileRequest profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create user account.
    /// </summary>
    /// <param name="user">
    /// Consumer.
    /// </param>
    /// <returns>
    /// User ID.
    /// </returns>
    protected internal Task<Result<Guid>> CreateUserAsync(CreateUserRequest user);

    /// <summary>
    /// Delete account.
    /// </summary>
    /// <param name="accountProfileId">
    /// Account profile ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns></returns>
    protected internal Task<Result> DeleteAccountAsync(Guid accountProfileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get usernames that contain the given username in part or whole.
    /// </summary>
    /// <param name="username">
    /// Username.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// A list of matching usernames.
    /// </returns>
    protected internal Task<List<string>> GetSimilarUsernamesAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the ID of account matching the given email.
    /// </summary>
    /// <param name="email">Email address.</param>
    /// <returns>
    /// The account ID if there's a match, otherwise <see langword="null"/>.
    /// </returns>
    protected internal Task<string?> GetIdAsync(string email);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    protected internal Task<Guid?> GetProfileIdAsync(string email);

    /// <summary>
    /// Get user's credentials.
    /// </summary>
    /// <param name="userId">
    /// User ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns></returns>
    protected internal Task<UserCredentials?> GetUserCredentialsAsync(string userId, CancellationToken cancellationToken = default);
}
