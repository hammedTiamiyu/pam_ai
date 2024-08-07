using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Dto.TermsOfService;
using PAMAi.Application.Errors;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Services;

internal class LegalService: ILegalService
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<LegalService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LegalService(ICurrentUser currentUser, ILogger<LegalService> logger, IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AcceptTermsOfServiceAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Could not accept Terms of Service. There is no logged in user");
            return Result.Failure(DefaultErrors.Unauthorised with
            {
                Description = "You must be logged in first.",
            });
        }

        UserTermsOfServiceConsent? userTermsOfServiceConsent = await _unitOfWork.UserTermsOfServiceConsents
            .FindAsync((Guid)_currentUser.UserProfileId!, id, cancellationToken);
        if (userTermsOfServiceConsent is null)
        {
            _logger.LogError("Could not accept Terms of Service. Terms of Service {Id} not found", id);
            return Result.Failure(DefaultErrors.NotFound with
            {
                Description = $"Terms of Service {id} not found.",
            });
        }

        if (userTermsOfServiceConsent.AcceptedDateUtc is not null)
        {
            _logger.LogInformation("User {UserId} has already accepted the Terms of Service {Id}. Skipped update",
                _currentUser.UserId,
                id);
            return Result.Success();
        }

        userTermsOfServiceConsent.AcceptedDateUtc = DateTimeOffset.Now;
        await _unitOfWork.CompleteAsync(cancellationToken);
        _logger.LogInformation("User {UserId} accepted the Terms of Service {Id}", _currentUser.UserId, id);

        return Result.Success();
    }

    public async Task<Result<int>> CreateTermsOfServiceAsync(CreateTermsOfServiceRequest terms, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating Terms of Service {@ToS}. Checking version...", terms);
        bool isVersionInUse = await _unitOfWork.TermsOfService.IsVersionInUseAsync(terms.Version, cancellationToken);
        if (isVersionInUse)
        {
            _logger.LogError("Cannot create Terms of Service. Version {Version} is in use", terms.Version);
            return Result<int>.Failure(TermsOfServiceErrors.DuplicateVersion with
            {
                Description = $"Version {terms.Version} is already in use by another Terms of Service.",
            });
        }

        _logger.LogDebug("Version {Version} is not in use. Proceeding to create Terms of Service", terms.Version);
        TermsOfService termsOfService = terms.Adapt<TermsOfService>();
        termsOfService.CreatedUtc = DateTime.UtcNow;
        await _unitOfWork.TermsOfService.AddAsync(termsOfService, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        _logger.LogInformation("Terms of Service created. Id: {Id}", termsOfService.Id);
        await AddTermsOfServiceForEveryUserAsync(termsOfService.Id);

        return Result<int>.Success(termsOfService.Id);
    }

    public async Task<Result> DeleteTermsOfServiceAsync(int id, CancellationToken cancellationToken = default)
    {
        TermsOfService? termsOfService = await _unitOfWork.TermsOfService.FindAsync(id, cancellationToken);
        if (termsOfService is null)
        {
            _logger.LogError("Could not delete Terms of Service. Terms of Service {Id} not found", id);
            return Result.Failure(DefaultErrors.NotFound with
            {
                Description = $"Terms of Service {id} not found.",
            });
        }

        _unitOfWork.TermsOfService.Remove(termsOfService);
        await _unitOfWork.CompleteAsync(cancellationToken);
        _logger.LogInformation("Terms of Service {Id} deleted", id);

        return Result.Success();
    }

    public async Task<Result<ReadTermsOfServiceResponse?>> GetCurrentTermsOfServiceAsync(CancellationToken cancellationToken = default)
    {
        TermsOfService? term = await _unitOfWork.TermsOfService.GetCurrentAsync(cancellationToken);
        ReadTermsOfServiceResponse? result = term.Adapt<ReadTermsOfServiceResponse?>();
        _logger.LogInformation("Fetched the current terms of service. Found: {Found}", result is not null);

        return Result<ReadTermsOfServiceResponse?>.Success(result);
    }

    public async Task<Result<PagedList<PreviewTermsOfServiceResponse>>> GetTermsOfServiceAsync(PaginationParameters paginationParams, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving a history of terms of service. Parameters: {@Params}", paginationParams);
        PagedList<TermsOfService> terms = await _unitOfWork.TermsOfService.GetAsync(paginationParams, cancellationToken);
        PagedList<PreviewTermsOfServiceResponse> result = terms.Adapt<PreviewTermsOfServiceResponse>();

        _logger.LogInformation("{Count} terms of service retrieved", result.Data.Count);
        return Result<PagedList<PreviewTermsOfServiceResponse>>.Success(result);
    }

    public async Task<Result<ReadTermsOfServiceResponse>> GetTermsOfServiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        TermsOfService? term = await _unitOfWork.TermsOfService.FindAsync(id, cancellationToken);
        if (term is null)
        {
            _logger.LogError("Terms of Service {Id} not found", id);
            return Result<ReadTermsOfServiceResponse>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Terms of Service with Id {id} not found.",
            });
        }

        ReadTermsOfServiceResponse result = term.Adapt<ReadTermsOfServiceResponse>();
        _logger.LogInformation("Terms of Service {Id} found", id);

        return Result<ReadTermsOfServiceResponse>.Success(result);
    }

    private async Task AddTermsOfServiceForEveryUserAsync(int termsOfServiceId)
    {
        IEnumerable<UserProfile> allUserProfiles = await _unitOfWork.UserProfiles.GetAsync();
        foreach (var userProfile in allUserProfiles)
        {
            UserTermsOfServiceConsent userTermsOfServiceConsent = new()
            {
                UserProfileId = userProfile.Id,
                TermsOfServiceId = termsOfServiceId,
                AcceptedDateUtc = null,
            };
            await _unitOfWork.UserTermsOfServiceConsents.AddAsync(userTermsOfServiceConsent);
        }
        await _unitOfWork.CompleteAsync();
        _logger.LogInformation("Added Terms of Service consent for TermsOfService {Id} with an 'unaccepted' status for every user. Every user will have to manually accept it",
            termsOfServiceId);
    }
}
