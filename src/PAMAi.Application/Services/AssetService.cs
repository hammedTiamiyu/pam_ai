﻿using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Asset;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Errors;
using PAMAi.Application.Resources;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Services;

internal class AssetService: IAssetService
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AssetService> _logger;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public AssetService(
        IAccountService accountService,
        ICurrentUser currentUser,
        ILogger<AssetService> logger,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _accountService = accountService;
        _currentUser = currentUser;
        _logger = logger;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> CreateAsync(CreateAssetRequest asset, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot create asset. There is no authenticated user");
            return Result<Guid>.Failure(DefaultErrors.Unauthorised);
        }

        var assetUserId = await CreateAssetUserAsync(asset, cancellationToken);
        if (assetUserId is null)
        {
            _logger.LogError("Returned user ID after user creation is null");
            return Result<Guid>.Failure(AssetErrors.UnableToCreate with
            {
                Description = "Unable to create asset owner.",
            });
        }

        var assetId = await CreateAssetAsync(asset, (Guid)assetUserId, cancellationToken);
        if (assetId is null)
            return Result<Guid>.Failure(AssetErrors.UnableToCreate);

        _logger.LogInformation("Account {Id} created asset {AssetId} and owner {OwnerId}",
            _currentUser.UserId,
            assetId,
            assetUserId);
        return Result<Guid>.Success((Guid)assetId);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot delete asset. There is no authenticated user");
            return Result.Failure(DefaultErrors.Unauthorised);
        }

        Asset? asset = await _unitOfWork.Assets.FindAsync(id, cancellationToken);
        if (asset is null)
        {
            _logger.LogError("Asset {Id} not found", id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Asset {id} not found.",
            });
        }

        if (!CurrentUserIsAssetInstaller(asset.InstallerProfile.UserId.ToString()))
        {
            _logger.LogError("Account {Id} attempted to delete asset {AssetId} which they didn't install",
                _currentUser.UserId,
                id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Forbidden with
            {
                Description = "You do not have sufficient rights.",
            });
        }

        _unitOfWork.Assets.Remove(asset);
        bool userOwnsMultipleAssets = await UserHasMultipleAssetsAsync(asset.OwnerProfileId, cancellationToken);
        if (!userOwnsMultipleAssets)
        {
            // Note: Because of the cascading relationship between UserProfile and Asset, deleting the UserProfile
            // already deletes the Asset.
            await _accountService.DeleteAccountAsync(asset.OwnerProfile.UserId.ToString(), cancellationToken);
        }
        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Installer {Id} deleted asset {Id}", _currentUser.UserId, id);
        return Result.Success();
    }

    public async Task<Result<PagedList<PreviewAssetResponse>>> GetAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot view assets. There is no authenticated user");
            return Result<PagedList<PreviewAssetResponse>>.Failure(DefaultErrors.Unauthorised);
        }

        PagedList<Asset> asset = await _unitOfWork.Assets.GetAsync(_currentUser.UserId!, paginationParameters, cancellationToken);
        PagedList<PreviewAssetResponse> response = asset.Adapt<PreviewAssetResponse>(PreviewAssetResponse.FromAsset);

        _logger.LogInformation("User {Id} fetched assets they created. Params: {@Params}",
            _currentUser.UserId,
            paginationParameters);
        return Result<PagedList<PreviewAssetResponse>>.Success(response);
    }

    public async Task<Result<ReadAssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot view asset. There is no authenticated user");
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Unauthorised);
        }

        Asset? asset = await _unitOfWork.Assets.FindAsync(id, cancellationToken);
        if (asset is null)
        {
            _logger.LogError("Asset {Id} not found", id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Asset {id} not found.",
            });
        }

        if (!CurrentUserIsAssetInstaller(asset.InstallerProfile.UserId.ToString()))
        {
            _logger.LogError("Account {Id} attempted to view asset {AssetId} which they didn't install",
                _currentUser.UserId,
                id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Forbidden);
        }

        ReadAssetResponse response = await TransformToReadResponseAsync(asset, cancellationToken);
        _logger.LogInformation("Account {AccountId} fetched asset {Id}", _currentUser.UserId, id);

        return Result<ReadAssetResponse>.Success(response);
    }

    public async Task<Result<ReadAssetResponse>> UpdateAsync(Guid id, UpdateAssetRequest updatedAsset, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot update asset. There is no authenticated user");
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Unauthorised);
        }

        Asset? asset = await _unitOfWork.Assets.FindAsync(id, cancellationToken);
        if (asset is null)
        {
            _logger.LogError("Asset {Id} not found", id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Asset {id} not found.",
            });
        }

        if (!CurrentUserIsAssetInstaller(asset.InstallerProfile.UserId.ToString()))
        {
            _logger.LogError("Account {Id} attempted to update asset {AssetId} which they didn't install",
                _currentUser.UserId,
                id);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Forbidden);
        }

        updatedAsset.Adapt(asset);
        asset.LastModifiedUtc = DateTimeOffset.Now;
        await _unitOfWork.CompleteAsync(cancellationToken);

        ReadAssetResponse response = await TransformToReadResponseAsync(asset, cancellationToken);
        _logger.LogInformation("Installer {AccountId} updated asset {Id}", _currentUser.UserId, id);

        return Result<ReadAssetResponse>.Success(response);
    }

    public async Task<Result> InviteAssetUserAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot invite user. There is no authenticated user");
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Unauthorised);
        }

        Asset? asset = await _unitOfWork.Assets.FindAsync(assetId, cancellationToken);
        if (asset is null)
        {
            _logger.LogError("Asset {Id} not found", assetId);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Asset {assetId} not found.",
            });
        }

        if (!CurrentUserIsAssetInstaller(asset.InstallerProfile.UserId.ToString()))
        {
            _logger.LogError("Account {Id} attempted to invite user of asset {AssetId}, which they didn't install",
                _currentUser.UserId,
                assetId);
            return Result<ReadAssetResponse>.Failure(DefaultErrors.Forbidden);
        }

        UserCredentials? credentials = await _accountService.GetUserCredentialsAsync(asset.OwnerProfile.UserId.ToString(), cancellationToken);
        if (credentials is null)
        {
            _logger.LogError("Cannot invite user. Credentials not found");
            return Result.Failure(AssetErrors.InviteUserFailed);
        }

        Result result = await SendInviteNotificationToUserAsync(
            asset.OwnerProfile.UserId.ToString(),
            credentials,
            cancellationToken);

        return result.IsSuccess
            ? result
            : Result.Failure(AssetErrors.InviteUserFailed with { InnerError = result.Error });
    }

    private static string CreateUsernameFromName(string firstName, string lastName)
    {
        firstName = firstName.Replace(" ", "");
        lastName = lastName.Replace(" ", "");

        return $"{firstName.ToLower()}{lastName.ToLower()}";
    }

    private static (string FirstName, string LastName) GetNames(string fullName)
    {
        string[] names = fullName.Split(' ');
        string firstName = names[0];
        if (names.Length == 1)
            return (firstName, string.Empty);
        string lastName = names[1..].Aggregate((str1, str2) => str1 + " " + str2);

        return (firstName, lastName);
    }

    private bool CurrentUserIsAssetInstaller(string assetInstaller)
    {
        return string.Equals(assetInstaller, _currentUser.UserId, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<Guid?> CreateAssetUserAsync(CreateAssetRequest asset, CancellationToken cancellationToken = default)
    {
        (string firstName, string lastName) = GetNames(asset.OwnerName);
        Guid? userProfileId = await _accountService.GetProfileIdAsync(asset.Email);
        if (userProfileId is not null)
        {
            _logger.LogInformation("An account already exists for {Email}. Adding them to user role", asset.Email);
            string? userId = await _accountService.GetIdAsync(asset.Email);
            await _accountService.AddAccountToRoleAsync(userId!, Domain.Enums.ApplicationRole.User);

            return userProfileId;
        }

        string username = CreateUsernameFromName(firstName, lastName);
        username = await GetUniqueUsernameAsync(username, cancellationToken);
        CreateUserRequest user = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Email = asset.Email,
            PhoneNumber = asset.PhoneNumber,
            Password = asset.Password,
        };
        Result<Guid> userCreationResult = await _accountService.CreateUserAsync(user);
        _logger.LogDebug("Created asset user result for {FullName}. Result: {@Result}",
            $"{user.FirstName} {user.LastName}",
            userCreationResult);

        return userCreationResult.Data;
    }

    private async Task<Guid?> CreateAssetAsync(CreateAssetRequest asset, Guid ownerId, CancellationToken cancellationToken = default)
    {
        Asset newAsset = asset.Adapt<Asset>();
        UserProfile? installerProfile = await _unitOfWork.UserProfiles.FindAsync(_currentUser.UserId!, cancellationToken);
        UserProfile? ownerProfile = await _unitOfWork.UserProfiles.FindAsync(ownerId, cancellationToken);
        newAsset.InstallerProfile = installerProfile!;
        newAsset.OwnerProfile = ownerProfile!;
        newAsset.CreatedUtc = DateTimeOffset.Now;

        try
        {
            await _unitOfWork.Assets.AddAsync(newAsset, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while saving the asset. Message: {Message}. Rolling back user creation", exception.Message);
            await _accountService.DeleteAccountAsync(ownerId);
            return null;
        }

        return newAsset.Id;
    }

    /// <summary>
    /// Checks the database for matching usernames. If there's an exact match,
    /// appends numbers to the username to make it unique.
    /// </summary>
    /// <param name="username">Username.</param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// The username which is surely unique.
    /// </returns>
    private async Task<string> GetUniqueUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        int append = 1;
        string tempUsername = username;
        List<string> similarUsernames = await _accountService.GetSimilarUsernamesAsync(username, cancellationToken);

        while (similarUsernames.Contains(tempUsername, StringComparer.OrdinalIgnoreCase))
        {
            tempUsername = username + append;
            append++;
        }

        _logger.LogTrace("Unique username created. Initial value: {Initial}. Unique value: {Unique}",
            username,
            tempUsername);

        return tempUsername;
    }

    private async Task<Result> SendInviteNotificationToUserAsync(string userId, UserCredentials userCredentials, CancellationToken cancellationToken = default)
    {
        NotificationContents notificationContents = new()
        {
            RecipientUserId = userId,
            Sms = new NotificationContents.SmsContent()
            {
                Message = string.Format(SmsMessages.InviteAssetUser, userCredentials.Email, userCredentials.Password),
            }
        };

        return await _notificationService.SendAsync(notificationContents, Domain.Enums.NotificationChannels.Sms, cancellationToken);
    }

    private async Task<bool> UserHasMultipleAssetsAsync(Guid userProfileId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Assets.UserHasMultipleAssetsAsync(userProfileId, cancellationToken);
    }

    private async Task<ReadAssetResponse> TransformToReadResponseAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        Result<ReadProfileResponse> result = await _accountService.GetProfileAsync(asset.OwnerProfile.UserId.ToString(), cancellationToken);
        ReadProfileResponse? ownerProfile = result.Data;
        ReadAssetResponse response = asset.Adapt<ReadAssetResponse>();
        response.PhoneNumber = ownerProfile?.PhoneNumber;
        response.Email = ownerProfile?.Email;
        response.OwnerName = $"{ownerProfile?.FirstName} {ownerProfile?.LastName}".Trim();

        return response;
    }
}