using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal sealed class UserProfileRepository: Repository<UserProfile, Guid>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public async Task<UserProfile?> FindAsync(string userId, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        UserProfile? user = await DbContext.UserProfiles
            .Where(u => u.UserId.ToString().ToUpper() == userId.ToUpper())
            .Include(u => u.State.Country)
            .FirstOrDefaultAsync();
        watch.Stop();

        Logger.LogDebug(
            "Searched {Entity} records for entity with user ID {UserId} in {Time} ms. Found: {found}",
            nameof(UserProfile),
            userId,
            watch.ElapsedMilliseconds,
            user is not null);

        return user;
    }
}
