using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class FlatRepository : IFlatRepository
{
    private readonly AppDbContext _dbContext;

    public FlatRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Flat>> GetByResidentIdAsync(Guid residentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Flats
            .Where(f => f.ResidentUserId == residentId)
            .Include(f => f.Building)
            .OrderBy(f => f.Number)
            .ToListAsync(cancellationToken);
    }
}
