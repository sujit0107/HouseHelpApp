using HouseHelp.Domain.Entities;

namespace HouseHelp.Domain.Repositories;

public interface IDisputeRepository
{
    Task<Dispute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Dispute>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Dispute dispute, CancellationToken cancellationToken = default);
    Task UpdateAsync(Dispute dispute, CancellationToken cancellationToken = default);
}
