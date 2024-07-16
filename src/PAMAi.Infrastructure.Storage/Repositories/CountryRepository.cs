using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal sealed class CountryRepository: Repository<Country, int>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public async Task<Country?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        var country = await DbContext.Countries
            .Where(c => c.Name.ToUpper() == name.ToUpper())
            .Include(c => c.States)
            .FirstOrDefaultAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Fetched country matching '{Name}' in {Time} ms. Found: {Found}",
            name,
            watch.ElapsedMilliseconds,
            country is not null);

        return country;
    }
}
