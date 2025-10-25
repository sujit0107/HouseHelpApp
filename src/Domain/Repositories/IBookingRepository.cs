using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByResidentAsync(Guid residentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByHelperAndStateAsync(Guid helperId, BookingState state, CancellationToken cancellationToken = default);
}
