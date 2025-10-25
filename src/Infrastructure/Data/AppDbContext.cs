using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Society> Societies => Set<Society>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Flat> Flats => Set<Flat>();
    public DbSet<User> Users => Set<User>();
    public DbSet<HelperProfile> HelperProfiles => Set<HelperProfile>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingEvent> BookingEvents => Set<BookingEvent>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Dispute> Disputes => Set<Dispute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
        modelBuilder.Entity<HelperProfile>().Property(x => x.Skills).HasColumnType("text[]");
        modelBuilder.Entity<HelperProfile>().Property(x => x.Languages).HasColumnType("text[]");
        modelBuilder.Entity<HelperProfile>().HasIndex(x => x.KycStatus);
        modelBuilder.Entity<Availability>().HasIndex(x => new { x.HelperId, x.Date, x.Start, x.End });
        modelBuilder.Entity<Booking>().Property(x => x.RowVersion).IsRowVersion();
        modelBuilder.Entity<Booking>().HasIndex(x => new { x.HelperId, x.StartAt, x.EndAt });
        modelBuilder.Entity<Booking>().HasIndex(x => new { x.ResidentId, x.CreatedAt });
        modelBuilder.Entity<Booking>().HasIndex(x => new { x.State, x.StartAt });
    }
}
