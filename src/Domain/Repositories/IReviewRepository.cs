using HouseHelp.Domain.Entities;

namespace HouseHelp.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
}
