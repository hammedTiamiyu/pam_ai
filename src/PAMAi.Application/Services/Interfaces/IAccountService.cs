using PAMAi.Application.Dto.Account;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for account operations.
/// </summary>
public interface IAccountService
{
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
    /// Create user account.
    /// </summary>
    /// <param name="user">
    /// Consumer.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the request.
    /// </param>
    /// <returns></returns>
    Task<Result> CreateUserAsync(CreateUserRequest user, CancellationToken cancellationToken = default);

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
    Task<Result<ReadProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default);
}
