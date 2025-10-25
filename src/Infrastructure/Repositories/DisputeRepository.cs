using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class DisputeRepository : IDisputeRepository
{
    private readonly AppDbContext _dbContext;

    public DisputeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Dispute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Disputes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Dispute>> GetOpenAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Disputes.Where(d => d.Status != "Resolved").ToListAsync(cancellationToken);

    public async Task AddAsync(Dispute dispute, CancellationToken cancellationToken = default)
        => await _dbContext.Disputes.AddAsync(dispute, cancellationToken);

    public Task UpdateAsync(Dispute dispute, CancellationToken cancellationToken = default)
    {
        _dbContext.Disputes.Update(dispute);
        return Task.CompletedTask;
    }
}
