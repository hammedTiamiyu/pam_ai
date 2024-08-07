using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal class UserTermsOfServiceConsentRepository: Repository<UserTermsOfServiceConsent>, IUserTermsOfServiceConsentRepository
{
    public UserTermsOfServiceConsentRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }
}
