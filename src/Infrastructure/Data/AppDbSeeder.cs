using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Infrastructure.Data;

public class AppDbSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AppDbSeeder> _logger;

    public AppDbSeeder(AppDbContext dbContext, ILogger<AppDbSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Skipping seed - users already exist");
            return;
        }

        var now = DateTimeOffset.UtcNow;

        var society = new Society
        {
            Id = Guid.Parse("8aa58fd0-1308-44f5-9c6d-2d6e6e4f27c1"),
            Name = "Azure Heights",
            Address = "1 Residency Road, Bengaluru",
            GeoFenceWkt = "POLYGON((77.5946 12.9716,77.5950 12.9716,77.5950 12.9720,77.5946 12.9720,77.5946 12.9716))",
            CreatedAt = now
        };

        var building = new Building
        {
            Id = Guid.Parse("19df7977-1ed3-46e1-82b0-4c036c529dbe"),
            SocietyId = society.Id,
            Name = "Block A"
        };

        var resident = new User
        {
            Id = Guid.Parse("c7a3f42a-9b36-4e22-9aac-5f8a4731d710"),
            Phone = "+910000000001",
            Name = "Riya Resident",
            Role = UserRole.Resident,
            Email = "riya.resident@example.com",
            IsActive = true,
            CreatedAt = now
        };

        var helper = new User
        {
            Id = Guid.Parse("3f0b0f4e-a2ab-4d1e-a52f-9a99a0f267a4"),
            Phone = "+910000000010",
            Name = "Harsh Helper",
            Role = UserRole.Helper,
            Email = "harsh.helper@example.com",
            IsActive = true,
            CreatedAt = now
        };

        var helperProfile = new HelperProfile
        {
            Id = helper.Id,
            UserId = helper.Id,
            Skills = new[] { "Cleaning", "Cooking" },
            Languages = new[] { "English", "Hindi" },
            ExperienceYears = 5,
            BaseRatePerHour = 400,
            RatingAvg = 4.7,
            JobsDone = 128,
            KycStatus = KycStatus.Approved,
            KycDocUrl = "https://example.com/kyc/harsh.pdf"
        };

        var flat = new Flat
        {
            Id = Guid.Parse("e86903aa-15f5-4582-8fb4-4ea091d77e2b"),
            BuildingId = building.Id,
            Number = "A-101",
            ResidentUserId = resident.Id
        };

        var availability = new Availability
        {
            Id = Guid.Parse("e9fb78ad-7de4-4a11-b42e-d8e2f358f63d"),
            HelperId = helperProfile.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            Start = new TimeOnly(9, 0),
            End = new TimeOnly(13, 0),
            IsRecurring = false,
            IsActive = true
        };

        await _dbContext.Societies.AddAsync(society, cancellationToken);
        await _dbContext.Buildings.AddAsync(building, cancellationToken);
        await _dbContext.Users.AddRangeAsync(resident, helper, cancellationToken);
        await _dbContext.HelperProfiles.AddAsync(helperProfile, cancellationToken);
        await _dbContext.Flats.AddAsync(flat, cancellationToken);
        await _dbContext.Availabilities.AddAsync(availability, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded demo data for resident UI");
    }
}
