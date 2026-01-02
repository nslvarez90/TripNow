using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Infrastructure.Data.Entities;

namespace TripNow.Infrastructure.Data.Configurations;

public class IdempotencyEntryConfiguration : IEntityTypeConfiguration<IdempotencyEntry>
{
    public void Configure(EntityTypeBuilder<IdempotencyEntry> builder)
    {
        builder.ToTable("IdempotencyKeys");

        builder.HasKey(x => x.IdempotencyKey);

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(255);

        builder.Property(x => x.RequestHash)
            .HasMaxLength(64);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_IdempotencyKeys_CreatedAt");
    }
}


