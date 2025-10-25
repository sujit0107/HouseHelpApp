namespace HouseHelp.Domain.Entities;

public class Society
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string GeoFenceWkt { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<Building> Buildings { get; set; } = new List<Building>();
}
