namespace HouseHelp.Domain.Entities;

public class Building
{
    public Guid Id { get; set; }
    public Guid SocietyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Society? Society { get; set; }
    public ICollection<Flat> Flats { get; set; } = new List<Flat>();
}
