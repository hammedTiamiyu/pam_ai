using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal class LegalContractRepository: Repository<LegalContract, int>, ILegalContractRepository
{
    public LegalContractRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }
}
