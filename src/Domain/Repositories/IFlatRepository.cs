using HouseHelp.Domain.Entities;

namespace HouseHelp.Domain.Repositories;

public interface IFlatRepository
{
    Task<IReadOnlyList<Flat>> GetByResidentIdAsync(Guid residentId, CancellationToken cancellationToken = default);
}
