﻿using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Asset;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Errors;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Services;

internal class AssetService: IAssetService
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AssetService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AssetService(IAccountService accountService, ICurrentUser currentUser, ILogger<AssetService> logger, IUnitOfWork unitOfWork)
    {
        _accountService = accountService;
        _currentUser = currentUser;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> CreateAsync(CreateAssetRequest asset, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.Any)
        {
            _logger.LogError("Cannot create asset. There is no authenticated user");
            return Result<Guid>.Failure(DefaultErrors.Unauthorised);
        }

        (string firstName, string lastName) = GetNames(asset.OwnerName);
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
        Result<string> userCreationResult = await _accountService.CreateUserAsync(user);
        if (userCreationResult.Data is null)
        {
            _logger.LogError("Returned user ID after user creation is null");
            return Result<Guid>.Failure(AssetErrors.UnableToCreate);
        }

        var assetId = await CreateAssetAsync(asset, userCreationResult.Data, cancellationToken);
        if (assetId is null)
            return Result<Guid>.Failure(AssetErrors.UnableToCreate);

        return Result<Guid>.Success((Guid)assetId);
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedListResponse<PreviewAssetResponse>>> GetAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ReadAssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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

    private static string CreateUsernameFromName(string firstName, string lastName)
    {
        firstName = firstName.Replace(" ", "");
        lastName = lastName.Replace(" ", "");

        return $"{firstName.ToLower()}{lastName.ToLower()}";
    }

    private async Task<Guid?> CreateAssetAsync(CreateAssetRequest asset, string ownerId, CancellationToken cancellationToken = default)
    {
        Asset newAsset = asset.Adapt<Asset>();
        newAsset.InstallerId = _currentUser.UserId!;
        newAsset.OwnerId = ownerId;
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
}