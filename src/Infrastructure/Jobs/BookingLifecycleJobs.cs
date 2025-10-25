using HouseHelp.Application.Bookings;
using HouseHelp.Application.Common;
using HouseHelp.Application.Payments;
using HouseHelp.Contracts.Payments;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HouseHelp.Infrastructure.Jobs;

public class BookingLifecycleJobs
{
    private readonly IBookingRepository _bookings;
    private readonly BookingService _bookingService;
    private readonly PaymentService _paymentService;
    private readonly ILogger<BookingLifecycleJobs> _logger;
    private readonly BookingOptions _options;

    public BookingLifecycleJobs(
        IBookingRepository bookings,
        BookingService bookingService,
        PaymentService paymentService,
        ILogger<BookingLifecycleJobs> logger,
        IOptions<BookingOptions> options)
    {
        _bookings = bookings;
        _bookingService = bookingService;
        _paymentService = paymentService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task ExpirePendingAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cutoff = DateTimeOffset.UtcNow.AddSeconds(-_options.RequestExpirySeconds);
        var pending = await _bookings.GetPendingExpiredAsync(cutoff, cancellationToken);
        if (pending.Count == 0)
        {
            return;
        }

        foreach (var booking in pending)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogInformation("Expiring booking {BookingId}", booking.Id);
                await _bookingService.CancelAsync(booking.Id, booking.ResidentId, "Auto-expired", cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to expire booking {BookingId}", booking.Id);
            }
        }
    }

    public async Task CapturePaymentAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        await _paymentService.CaptureAsync(new PaymentCaptureRequestDto(bookingId), cancellationToken);
    }
}
