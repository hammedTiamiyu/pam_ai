using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="TermsOfService"/> entities in the database.
/// </summary>
public interface ITermsOfServiceRepository: IRepository<TermsOfService, int>
{
}
