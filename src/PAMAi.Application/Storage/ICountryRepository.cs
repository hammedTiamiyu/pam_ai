using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="Country"/> entities in the database.
/// </summary>
public interface ICountryRepository: IRepository<Country, int>
{
    /// <summary>
    /// Find country by name.
    /// </summary>
    /// <param name="name">
    /// Country name.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel operation.
    /// </param>
    /// <returns>
    /// <see cref="Country"/> if found, otherwise <see langword="null"/>.
    /// </returns>
    Task<Country?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
