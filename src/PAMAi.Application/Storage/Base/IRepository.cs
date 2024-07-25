using System.Linq.Expressions;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Domain.Entities.Base;

namespace PAMAi.Application.Storage.Base;

/// <summary>
/// Interface of a repository of <typeparamref name="TEntity"/> objects, having their
/// primary key data type as <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
/// <typeparam name="TKey">
/// Data type of the primary key.
/// </typeparam>
public interface IRepository<TEntity, TKey>: IRepository<TEntity> where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Find by its identifier.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The <typeparamref name="TEntity"/> entity if it exists, otherwise <see langword="null"/>.</returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for a repository of <typeparamref name="TEntity"/> objects.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public interface IRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Return all records.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// An enumerable containing all records in the table.
    /// </returns>
    Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Return a paged list of <typeparamref name="TEntity"/> items.
    /// </summary>
    /// <param name="paginationParams">
    /// Pagination parameters.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>A paged list of <typeparamref name="TEntity"/>.</returns>
    Task<PagedList<TEntity>> GetAsync(PaginationParameters paginationParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find all records which satisfy the given condition.
    /// </summary>
    /// <param name="predicate">
    /// Predicate holding the condition.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A list of records that satisfy the given condition.
    /// </returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find all records which satisfy the given condition.
    /// </summary>
    /// <param name="predicate">
    /// Predicate holding the condition.
    /// </param>
    /// <param name="paginationParams">
    /// Pagination parameters.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>A paged list of <typeparamref name="TEntity"/> that satisfy the given condition.</returns>
    Task<PagedList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, PaginationParameters paginationParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first record of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token for cancelling operations.
    /// </param>
    /// <returns></returns>
    Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin tracking the entity so that it will be inserted in the database when
    /// <see cref="IUnitOfWork.CompleteAsync(CancellationToken)"/> is called.
    /// </summary>
    /// <param name="entity">
    /// Entity to track.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin tracking entities so that they will be inserted in the database when
    /// <see cref="IUnitOfWork.CompleteAsync(CancellationToken)"/> is called.
    /// </summary>
    /// <param name="entities">
    /// Entities to track.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove from the repository.
    /// </summary>
    /// <param name="entity">
    /// Entity to remove.
    /// </param>
    void Remove(TEntity entity);

    /// <summary>
    /// Remove from the repository.
    /// </summary>
    /// <param name="entities">
    /// Entities to remove.
    /// </param>
    void RemoveRange(IEnumerable<TEntity> entities);
}
