﻿using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories;

namespace PAMAi.Infrastructure.Storage;

internal sealed class UnitOfWork: IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private bool _disposedValue;

    public UnitOfWork(ApplicationDbContext dbContext, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _logger = logger;

        Assets = new AssetRepository(_dbContext, _logger);
        Countries = new CountryRepository(_dbContext, _logger);
        States = new StateRepository(_dbContext, _logger);
        TermsOfService = new TermsOfServiceRepository(_dbContext, _logger);
        UserProfiles = new UserProfileRepository(_dbContext, _logger);
        UserTermsOfServiceConsents = new UserTermsOfServiceConsentRepository(_dbContext, _logger);
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }

    public IAssetRepository Assets { get; }
    public ICountryRepository Countries { get; }
    public IStateRepository States { get; }
    public ITermsOfServiceRepository TermsOfService { get; }
    public IUserProfileRepository UserProfiles { get; }
    public IUserTermsOfServiceConsentRepository UserTermsOfServiceConsents { get; }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        int changesMade = await _dbContext.SaveChangesAsync(cancellationToken);
        watch.Stop();
        _logger.LogDebug("{count} records affected in the database in {time} ms", changesMade, watch.ElapsedMilliseconds);

        return changesMade;
    }

    #region Dispose

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                await _dbContext.DisposeAsync();
            }
        }
        _disposedValue = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
