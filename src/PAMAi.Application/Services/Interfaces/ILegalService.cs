using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Dto.TermsOfService;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for operations on the legal aspects (terms of use, policies, etc) of PAMAi.
/// </summary>
public interface ILegalService
{
    /// <summary>
    /// Accept a Terms of Service for the current logged-in user.
    /// </summary>
    /// <param name="id">
    /// Terms of Service's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> AcceptTermsOfServiceAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new Terms of Service.
    /// </summary>
    /// <param name="terms">
    /// Terms of Service.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation containing the ID of the newly created
    /// Terms of Service.
    /// </returns>
    Task<Result<int>> CreateTermsOfServiceAsync(CreateTermsOfServiceRequest terms, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete Terms of Service.
    /// </summary>
    /// <param name="id">
    /// Terms of Service ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> DeleteTermsOfServiceAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current terms of service in use.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The current terms of service if available, otherwise <see langword="null"/>.
    /// </returns>
    Task<Result<ReadTermsOfServiceResponse?>> GetCurrentTermsOfServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a history of the terms of service of the application.
    /// </summary>
    /// <param name="paginationParams">
    /// Pagination parameters.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// A history of terms of service.
    /// </returns>
    Task<Result<PagedList<PreviewTermsOfServiceResponse>>> GetTermsOfServiceAsync(PaginationParameters paginationParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a <see cref="TermsOfService"/> by its ID.
    /// </summary>
    /// <param name="id">
    /// Terms of Service's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// A result containing the Terms of Service.
    /// </returns>
    Task<Result<ReadTermsOfServiceResponse>> GetTermsOfServiceByIdAsync(int id, CancellationToken cancellationToken = default);
}
