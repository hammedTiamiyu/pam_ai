using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="LegalContract"/> entities in the database.
/// </summary>
public interface ILegalContractRepository: IRepository<LegalContract, int>
{
}
