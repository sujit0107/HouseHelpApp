using HouseHelp.Application.Bookings;
using HouseHelp.Application.Payments;
using HouseHelp.Contracts.Payments;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Infrastructure.Jobs;

public class BookingLifecycleJobs
{
    private readonly IBookingRepository _bookings;
    private readonly BookingService _bookingService;
    private readonly PaymentService _paymentService;
    private readonly ILogger<BookingLifecycleJobs> _logger;

    public BookingLifecycleJobs(IBookingRepository bookings, BookingService bookingService, PaymentService paymentService, ILogger<BookingLifecycleJobs> logger)
    {
        _bookings = bookings;
        _bookingService = bookingService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task ExpirePendingAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null || booking.State != BookingState.Requested)
        {
            return;
        }

        _logger.LogInformation("Expiring booking {BookingId}", bookingId);
        await _bookingService.CancelAsync(booking.Id, booking.ResidentId, "Auto-expired", cancellationToken);
    }

    public async Task CapturePaymentAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        await _paymentService.CaptureAsync(new PaymentCaptureRequestDto(bookingId), cancellationToken);
    }
}
