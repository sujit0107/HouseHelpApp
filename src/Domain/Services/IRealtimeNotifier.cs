using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Services;

public interface IRealtimeNotifier
{
    Task NotifyBookingAsync(Guid bookingId, BookingState state, CancellationToken cancellationToken = default);
    Task NotifyUserAsync(Guid userId, string eventName, object payload, CancellationToken cancellationToken = default);
    Task NotifyPaymentCapturedAsync(Guid bookingId, CancellationToken cancellationToken = default);
}
