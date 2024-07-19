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

    public override async Task<Country?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Finding {entity} record by key {key}", typeof(Country).Name, id);

        var watch = Stopwatch.StartNew();
        Country? entity = await DbContext.Countries
            .Where(c => c.Id == id)
            .Include(c => c.States)
            .FirstOrDefaultAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug(
            "Searched {entity} records for entity with key {key} in {time} ms. Found: {found}",
            typeof(Country).Name,
            id,
            watch.ElapsedMilliseconds,
            entity is not null);

        return entity;
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
