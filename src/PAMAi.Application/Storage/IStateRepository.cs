using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="State"/> entities in the database.
/// </summary>
public interface IStateRepository: IRepository<State, long>
{
    /// <summary>
    /// Find state by name.
    /// </summary>
    /// <param name="name">
    /// Name of state.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel operation.
    /// </param>
    /// <returns>
    /// A <see cref="State"/> if there's a match, otherwise <see langword="null"/>.
    /// </returns>
    Task<State?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
