using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly AppDbContext _dbContext;

    public AvailabilityRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Availability>> GetForHelperAsync(Guid helperId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Availabilities.Where(x => x.HelperId == helperId && x.IsActive).OrderBy(x => x.Date).ThenBy(x => x.Start).ToListAsync(cancellationToken);
    }

    public async Task UpsertRangeAsync(IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default)
    {
        foreach (var availability in availabilities)
        {
            var existing = await _dbContext.Availabilities.FirstOrDefaultAsync(x => x.Id == availability.Id, cancellationToken);
            if (existing is null)
            {
                await _dbContext.Availabilities.AddAsync(availability, cancellationToken);
            }
            else
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(availability);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HelperProfile>> SearchHelpersAsync(HelperSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.HelperProfiles.Include(h => h.User).AsQueryable();
        if (!string.IsNullOrEmpty(criteria.Skill))
        {
            query = query.Where(h => h.Skills.Contains(criteria.Skill));
        }

        if (criteria.PriceMin is not null)
        {
            query = query.Where(h => h.BaseRatePerHour >= criteria.PriceMin);
        }

        if (criteria.PriceMax is not null)
        {
            query = query.Where(h => h.BaseRatePerHour <= criteria.PriceMax);
        }

        var helpers = await query.ToListAsync(cancellationToken);
        return helpers;
    }
}
