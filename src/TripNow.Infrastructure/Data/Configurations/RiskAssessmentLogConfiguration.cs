using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Infrastructure.Data.Entities;

namespace TripNow.Infrastructure.Data.Configurations;

public class RiskAssessmentLogConfiguration : IEntityTypeConfiguration<RiskAssessmentLog>
{
    public void Configure(EntityTypeBuilder<RiskAssessmentLog> builder)
    {
        builder.ToTable("RiskAssessmentLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(x => x.BookingId)
            .HasDatabaseName("IX_RiskAssessmentLogs_BookingId");
    }
}


