using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Storage;
using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities.Base;
using PAMAi.Infrastructure.Storage.Contexts;

namespace PAMAi.Infrastructure.Storage.Repositories.Base;

internal abstract class Repository<TEntity, TKey>: Repository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public Repository(ApplicationDbContext context, ILogger<UnitOfWork> logger) : base(context, logger)
    {
    }

    public virtual async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Finding {entity} record by key {key}", typeof(TEntity).Name, id);

        var watch = Stopwatch.StartNew();
        TEntity? entity = await DbContext.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);
        watch.Stop();
        Logger.LogDebug(
            "Searched {entity} records for entity with key {key} in {time} ms. Found: {found}",
            typeof(TEntity).Name,
            id,
            watch.ElapsedMilliseconds,
            entity is not null);

        return entity;
    }

    public override Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
        DbContext.Set<TEntity>().OrderBy(e => e.Id).FirstOrDefaultAsync(cancellationToken);
}

internal abstract class Repository<TEntity>: IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly ILogger<UnitOfWork> Logger;

    public Repository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
    {
        DbContext = context;
        Logger = logger;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace(
            "Finding all {entity} records that satisfy the condition `{condition}`",
            typeof(TEntity).Name,
            predicate.Body.Print());

        var watch = Stopwatch.StartNew();
        var entities = await DbContext.Set<TEntity>()
            .Where(predicate)
            .ToListAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug(
            "Retrieved {count} {entity} records that satisfy the condition `{condition}` in {time} ms",
            entities.Count,
            typeof(TEntity).Name,
            predicate.Body.Print(),
            watch.ElapsedMilliseconds);

        return entities;
    }

    public virtual async Task<PagedList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, QueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace(
           "Finding {entity} records that satisfy the condition `{condition}`",
           typeof(TEntity).Name,
           predicate.Body.Print());

        var watch = Stopwatch.StartNew();
        var entities = await PagedList<TEntity>.CreateAsync(
            DbContext.Set<TEntity>().Where(predicate),
            queryParameters.Page,
            queryParameters.PageSize,
            cancellationToken);
        watch.Stop();
        Logger.LogDebug(
            "Retrieved page {page} containing {count} {entity} records that satisfy the condition `{condition}` in {time} ms",
            queryParameters.Page,
            entities.Count,
            typeof(TEntity).Name,
            predicate.Body.Print(),
            watch.ElapsedMilliseconds);

        return entities;
    }

    public virtual Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => DbContext.Set<TEntity>().FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Retrieving all {entity} records", typeof(TEntity).Name);

        var watch = Stopwatch.StartNew();
        var entities = await DbContext.Set<TEntity>().ToListAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Retrieved all {entity} records in {time} ms", typeof(TEntity).Name, watch.ElapsedMilliseconds);

        return entities;
    }

    public virtual async Task<PagedList<TEntity>> GetPagedListAsync(QueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace(
            "Retrieving paged list of {entity} records (page = {page}, page size = {size})",
            typeof(TEntity).Name,
            queryParameters.Page,
            queryParameters.PageSize);

        var watch = Stopwatch.StartNew();
        var entities = await PagedList<TEntity>.CreateAsync(
            DbContext.Set<TEntity>(),
            queryParameters.Page,
            queryParameters.PageSize,
            cancellationToken);
        watch.Stop();
        Logger.LogDebug(
            "Retrieved page {page} containing {number} {entity} records in {time} ms",
            queryParameters.Page,
            entities.Count,
            typeof(TEntity).Name,
            watch.ElapsedMilliseconds);

        return entities;
    }

    public void Remove(TEntity entity)
        => DbContext.Set<TEntity>().Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities)
        => DbContext.Set<TEntity>().RemoveRange(entities);
}
