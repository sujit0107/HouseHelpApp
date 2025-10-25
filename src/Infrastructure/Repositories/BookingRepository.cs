using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Bookings.Include(b => b.Events).Include(b => b.Payment).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _dbContext.Bookings.Update(booking);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Booking>> GetByResidentAsync(Guid residentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings.Where(b => b.ResidentId == residentId).OrderByDescending(b => b.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByHelperAndStateAsync(Guid helperId, BookingState state, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings.Where(b => b.HelperId == helperId && b.State == state).OrderBy(b => b.StartAt).ToListAsync(cancellationToken);
    }
}
