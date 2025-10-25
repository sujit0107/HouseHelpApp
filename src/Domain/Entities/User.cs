using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Name { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public HelperProfile? HelperProfile { get; set; }
}
