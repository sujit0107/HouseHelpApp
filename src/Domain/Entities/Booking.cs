using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ResidentId { get; set; }
    public Guid HelperId { get; set; }
    public Guid FlatId { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public decimal PriceEstimate { get; set; }
    public BookingState State { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? Notes { get; set; }
    public uint RowVersion { get; set; }

    public User? Resident { get; set; }
    public User? Helper { get; set; }
    public Flat? Flat { get; set; }
    public ICollection<BookingEvent> Events { get; set; } = new List<BookingEvent>();
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
    public Dispute? Dispute { get; set; }
}
