using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _dbContext;

    public ReviewRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Review?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        => _dbContext.Reviews.FirstOrDefaultAsync(r => r.BookingId == bookingId, cancellationToken);

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
        => await _dbContext.Reviews.AddAsync(review, cancellationToken);
}
