using HouseHelp.Contracts.Payments;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using HouseHelp.Domain.Services;
using HouseHelp.Domain.Services.Models;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Application.Payments;

public class PaymentService
{
    private readonly IPaymentRepository _payments;
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRazorpayClient _razorpayClient;
    private readonly IRealtimeNotifier _notifier;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository payments,
        IBookingRepository bookings,
        IUnitOfWork unitOfWork,
        IRazorpayClient razorpayClient,
        IRealtimeNotifier notifier,
        ILogger<PaymentService> logger)
    {
        _payments = payments;
        _bookings = bookings;
        _unitOfWork = unitOfWork;
        _razorpayClient = razorpayClient;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> CreateIntentAsync(CreatePaymentIntentRequestDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        var existing = await _payments.GetByBookingIdAsync(booking.Id, cancellationToken);
        if (existing is not null && !string.IsNullOrEmpty(existing.IntentId))
        {
            return Map(existing);
        }

        var intent = await _razorpayClient.CreateIntentAsync(new PaymentIntentRequest("INR", (long)(booking.PriceEstimate * 100), $"Booking {booking.Id}", new Dictionary<string, string>{{"bookingId", booking.Id.ToString()}}), cancellationToken);
        var payment = existing ?? new Payment { Id = Guid.NewGuid(), BookingId = booking.Id, Provider = "Razorpay", Currency = "INR", AmountMinor = (long)(booking.PriceEstimate * 100), CreatedAt = DateTimeOffset.UtcNow };
        payment.IntentId = intent.IntentId;
        payment.Status = intent.Status;
        if (existing is null)
        {
            await _payments.AddAsync(payment, cancellationToken);
        }
        else
        {
            await _payments.UpdateAsync(payment, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(payment);
    }

    public async Task<PaymentResponseDto> CaptureAsync(PaymentCaptureRequestDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.State is not BookingState.Completed and not BookingState.InProgress)
        {
            throw new InvalidOperationException("Cannot capture payment before completion");
        }

        var payment = await _payments.GetByBookingIdAsync(booking.Id, cancellationToken) ?? throw new InvalidOperationException("Payment not found");
        if (!string.IsNullOrEmpty(payment.CaptureId))
        {
            return Map(payment);
        }

        var capture = await _razorpayClient.CaptureAsync(payment.IntentId, payment.AmountMinor, cancellationToken);
        payment.CaptureId = capture.CaptureId;
        payment.CapturedAt = DateTimeOffset.UtcNow;
        payment.Status = capture.Status;
        booking.State = BookingState.Paid;
        booking.UpdatedAt = DateTimeOffset.UtcNow;
        await _payments.UpdateAsync(payment, cancellationToken);
        await _bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifier.NotifyPaymentCapturedAsync(booking.Id, cancellationToken);
        return Map(payment);
    }

    public async Task<PaymentResponseDto> RefundAsync(PaymentRefundRequestDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        var payment = await _payments.GetByBookingIdAsync(booking.Id, cancellationToken) ?? throw new InvalidOperationException("Payment not found");
        if (payment.RefundedAt is not null)
        {
            return Map(payment);
        }

        var refund = await _razorpayClient.RefundAsync(payment.IntentId, payment.AmountMinor, request.Reason, cancellationToken);
        payment.RefundedAt = DateTimeOffset.UtcNow;
        payment.Status = refund.Status;
        payment.RefundReason = request.Reason;
        await _payments.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(payment);
    }

    private static PaymentResponseDto Map(Payment payment)
        => new(payment.BookingId, payment.Provider, payment.Status, payment.IntentId, payment.CaptureId, payment.InvoiceUrl);
}
