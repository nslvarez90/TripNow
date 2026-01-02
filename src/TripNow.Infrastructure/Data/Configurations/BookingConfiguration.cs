using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Core.Entities;
using TripNow.Core.Enums;

namespace TripNow.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.TripCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(BookingStatus.PendingRiskCheck);

        builder.Property(b => b.Reason)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(b => b.CorrelationId)
            .IsRequired();

        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.CreatedAt);
        builder.HasIndex(b => b.CorrelationId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Core.Entities;

namespace TripNow.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.TripCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.Reason)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_Bookings_Status");

        builder.HasIndex(b => b.CreatedAt)
            .HasDatabaseName("IX_Bookings_CreatedAt");

        builder.HasIndex(b => b.CorrelationId)
            .HasDatabaseName("IX_Bookings_CorrelationId");
    }
}


