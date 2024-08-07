using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal class UserLegalContractConsentRepository: Repository<UserLegalContractConsent>, IUserLegalContractConsentRepository
{
    public UserLegalContractConsentRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }
}
