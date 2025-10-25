using System;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace HouseHelp.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("HouseHelp.Domain.Entities.Availability", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateOnly>("Date").HasColumnType("date");
                b.Property<Guid>("HelperId").HasColumnType("uuid");
                b.Property<bool>("IsActive").HasColumnType("boolean");
                b.Property<bool>("IsRecurring").HasColumnType("boolean");
                b.Property<string?>("RecurrenceRule").HasColumnType("text");
                b.Property<TimeOnly>("Start").HasColumnType("time without time zone");
                b.Property<TimeOnly>("End").HasColumnType("time without time zone");
                b.HasKey("Id");
                b.HasIndex("HelperId", "Date", "Start", "End");
                b.ToTable("Availabilities");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.Booking", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("ResidentId").HasColumnType("uuid");
                b.Property<Guid>("HelperId").HasColumnType("uuid");
                b.Property<Guid>("FlatId").HasColumnType("uuid");
                b.Property<string>("ServiceType").HasColumnType("text");
                b.Property<DateTimeOffset>("StartAt").HasColumnType("timestamp with time zone");
                b.Property<DateTimeOffset>("EndAt").HasColumnType("timestamp with time zone");
                b.Property<decimal>("PriceEstimate").HasColumnType("numeric");
                b.Property<int>("State").HasColumnType("integer");
                b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
                b.Property<string?>("Notes").HasColumnType("text");
                b.Property<uint>("RowVersion").IsRowVersion().HasColumnType("xid");
                b.HasKey("Id");
                b.HasIndex("FlatId");
                b.HasIndex("HelperId", "StartAt", "EndAt");
                b.HasIndex("ResidentId", "CreatedAt");
                b.HasIndex("State", "StartAt");
                b.ToTable("Bookings");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.BookingEvent", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("BookingId").HasColumnType("uuid");
                b.Property<int>("From").HasColumnType("integer");
                b.Property<int>("To").HasColumnType("integer");
                b.Property<DateTimeOffset>("At").HasColumnType("timestamp with time zone");
                b.Property<Guid>("ActorId").HasColumnType("uuid");
                b.Property<string?>("Reason").HasColumnType("text");
                b.HasKey("Id");
                b.HasIndex("BookingId");
                b.ToTable("BookingEvents");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.HelperProfile", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<decimal>("BaseRatePerHour").HasColumnType("numeric");
                b.Property<int>("ExperienceYears").HasColumnType("integer");
                b.Property<int>("JobsDone").HasColumnType("integer");
                b.Property<int>("KycStatus").HasColumnType("integer");
                b.Property<string?>("KycDocUrl").HasColumnType("text");
                b.Property<double>("RatingAvg").HasColumnType("double precision");
                b.Property<string[]>("Languages").HasColumnType("text[]");
                b.Property<string[]>("Skills").HasColumnType("text[]");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("KycStatus");
                b.HasIndex("UserId").IsUnique();
                b.ToTable("HelperProfiles");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.Payment", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<long>("AmountMinor").HasColumnType("bigint");
                b.Property<Guid>("BookingId").HasColumnType("uuid");
                b.Property<string?>("CaptureId").HasColumnType("text");
                b.Property<DateTimeOffset?>("CapturedAt").HasColumnType("timestamp with time zone");
                b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<string>("Currency").HasColumnType("text");
                b.Property<string>("IntentId").HasColumnType("text");
                b.Property<string?>("InvoiceUrl").HasColumnType("text");
                b.Property<string>("Provider").HasColumnType("text");
                b.Property<string?>("RefundReason").HasColumnType("text");
                b.Property<DateTimeOffset?>("RefundedAt").HasColumnType("timestamp with time zone");
                b.Property<string>("Status").HasColumnType("text");
                b.HasKey("Id");
                b.HasIndex("BookingId").IsUnique();
                b.ToTable("Payments");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.Review", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("BookingId").HasColumnType("uuid");
                b.Property<int>("Rating").HasColumnType("integer");
                b.Property<Guid>("RateeId").HasColumnType("uuid");
                b.Property<Guid>("RaterId").HasColumnType("uuid");
                b.Property<string?>("Comment").HasColumnType("text");
                b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
                b.HasKey("Id");
                b.HasIndex("BookingId").IsUnique();
                b.ToTable("Reviews");
            });

            modelBuilder.Entity("HouseHelp.Domain.Entities.User", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<string?>("Email").HasColumnType("text");
                b.Property<bool>("IsActive").HasColumnType("boolean");
                b.Property<string?>("Name").HasColumnType("text");
                b.Property<string>("Phone").HasColumnType("text");
                b.Property<int>("Role").HasColumnType("integer");
                b.HasKey("Id");
                b.HasIndex("Phone").IsUnique();
                b.ToTable("Users");
            });
        }
    }
}
