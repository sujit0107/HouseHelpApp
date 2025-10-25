namespace HouseHelp.Contracts.Payments;

public record CreatePaymentIntentRequestDto(Guid BookingId);

public record PaymentCaptureRequestDto(Guid BookingId);

public record PaymentRefundRequestDto(Guid BookingId, string Reason);

public record PaymentResponseDto(Guid BookingId, string Provider, string Status, string? IntentId, string? CaptureId, string? InvoiceUrl);
