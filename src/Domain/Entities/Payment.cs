namespace HouseHelp.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Currency { get; set; } = "INR";
    public long AmountMinor { get; set; }
    public string IntentId { get; set; } = string.Empty;
    public string? CaptureId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CapturedAt { get; set; }
    public DateTimeOffset? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public string? InvoiceUrl { get; set; }
    public Booking? Booking { get; set; }
}
