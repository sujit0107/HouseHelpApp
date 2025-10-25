namespace HouseHelp.Domain.Entities;

public class Dispute
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid OpenedBy { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? Resolution { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public Booking? Booking { get; set; }
}
