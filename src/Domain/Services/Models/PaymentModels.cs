namespace HouseHelp.Domain.Services.Models;

public record PaymentIntentRequest(string Currency, long AmountMinor, string Description, IDictionary<string, string>? Metadata);

public record PaymentIntentResult(string IntentId, string ClientSecret, string Status);

public record PaymentCaptureResult(string CaptureId, string Status);

public record PaymentRefundResult(string RefundId, string Status);
