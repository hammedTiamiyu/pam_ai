using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal sealed class StateRepository: Repository<State, long>, IStateRepository
{
    public StateRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public async Task<State?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        var state = await DbContext.States
            .Where(s => s.Name.ToUpper() == name.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Fetched state matching '{Name}' in {Time} ms. Found: {Found}",
            name,
            watch.ElapsedMilliseconds,
            state is not null);

        return state;
    }
}
