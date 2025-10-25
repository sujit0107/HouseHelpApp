using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Services;
using HouseHelp.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace HouseHelp.Infrastructure.Realtime;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<BookingHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyBookingAsync(Guid bookingId, BookingState state, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"booking:{bookingId}").SendAsync("booking.state", new { bookingId, state }, cancellationToken);
    }

    public Task NotifyUserAsync(Guid userId, string eventName, object payload, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user:{userId}").SendAsync(eventName, payload, cancellationToken);
    }

    public Task NotifyPaymentCapturedAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"booking:{bookingId}").SendAsync("payment.captured", new { bookingId }, cancellationToken);
    }
}
