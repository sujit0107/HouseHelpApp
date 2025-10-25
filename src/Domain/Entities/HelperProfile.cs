using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Entities;

public class HelperProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string[] Skills { get; set; } = Array.Empty<string>();
    public string[] Languages { get; set; } = Array.Empty<string>();
    public int ExperienceYears { get; set; }
    public decimal BaseRatePerHour { get; set; }
    public double RatingAvg { get; set; }
    public int JobsDone { get; set; }
    public KycStatus KycStatus { get; set; }
    public string? KycDocUrl { get; set; }
    public User? User { get; set; }
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
}
