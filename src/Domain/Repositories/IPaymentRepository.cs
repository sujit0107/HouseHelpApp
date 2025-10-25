using HouseHelp.Domain.Entities;

namespace HouseHelp.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
}
