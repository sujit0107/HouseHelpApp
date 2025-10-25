using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HouseHelp.Realtime;

[Authorize]
public class BookingHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        await base.OnConnectedAsync();
    }

    public Task JoinBooking(Guid bookingId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, $"booking:{bookingId}");
    }
}
