namespace HouseHelp.Domain.Entities;

public class Availability
{
    public Guid Id { get; set; }
    public Guid HelperId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public bool IsActive { get; set; }
    public HelperProfile? Helper { get; set; }
}
