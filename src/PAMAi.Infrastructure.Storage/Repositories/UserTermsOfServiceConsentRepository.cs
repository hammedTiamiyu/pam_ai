using System.Diagnostics;
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

    public async Task<UserTermsOfServiceConsent?> FindAsync(Guid userProfileId,  int termsOfServiceId, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Fetching {Type} for user profile {Id} and Terms of Service {id}",
            typeof(UserTermsOfServiceConsent).FullName,
            userProfileId,
            termsOfServiceId);

        var watch = Stopwatch.StartNew();
        UserTermsOfServiceConsent? userTermsOfServiceConsent = await DbContext.UserTermsOfServiceConsents
            .Where(ut => ut.UserProfileId == userProfileId && ut.TermsOfServiceId == termsOfServiceId)
            .FirstOrDefaultAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Fetched {Type} for user profile {Id} and Terms of Service {id} in {Time} ms. Found: {Result}",
             typeof(UserTermsOfServiceConsent).FullName,
            userProfileId,
            termsOfServiceId,
            watch.ElapsedMilliseconds,
            userTermsOfServiceConsent is not null);

        return userTermsOfServiceConsent;
    }

    public async Task<bool> IsCurrentTermsOfServiceAcceptedAsync(Guid userProfileId, CancellationToken cancellationToken = default)
    {
        TermsOfService? currentTermsOfService = await TermsOfServiceRepository.GetCurrentTermsOfServiceAsync(DbContext, cancellationToken);
        if (currentTermsOfService is null)
        {
            return false;
        }

        bool isAccepted = await DbContext.UserTermsOfServiceConsents
            .Where(ut => ut.UserProfileId == userProfileId && ut.TermsOfServiceId == currentTermsOfService.Id)
            .Where(ut => ut.AcceptedDateUtc != null)
            .AnyAsync(cancellationToken);

        return isAccepted;
    }
}
