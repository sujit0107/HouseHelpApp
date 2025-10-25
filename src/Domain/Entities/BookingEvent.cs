using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Entities;

public class BookingEvent
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public BookingState From { get; set; }
    public BookingState To { get; set; }
    public DateTimeOffset At { get; set; }
    public Guid ActorId { get; set; }
    public string? Reason { get; set; }
    public Booking? Booking { get; set; }
}
