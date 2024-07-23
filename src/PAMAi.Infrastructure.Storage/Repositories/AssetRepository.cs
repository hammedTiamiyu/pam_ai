using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;

internal class AssetRepository: Repository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger) 
        : base(context, logger)
    {
    }
}
