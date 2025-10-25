using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        => _dbContext.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        => await _dbContext.Payments.AddAsync(payment, cancellationToken);

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _dbContext.Payments.Update(payment);
        return Task.CompletedTask;
    }
}
