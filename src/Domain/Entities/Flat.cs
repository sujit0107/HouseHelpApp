namespace HouseHelp.Domain.Entities;

public class Flat
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid ResidentUserId { get; set; }
    public Building? Building { get; set; }
}
