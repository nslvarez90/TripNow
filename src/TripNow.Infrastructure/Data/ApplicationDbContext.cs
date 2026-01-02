using Microsoft.EntityFrameworkCore;
using TripNow.Core.Entities;
using TripNow.Infrastructure.Data.Entities;

namespace TripNow.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<IdempotencyEntry> IdempotencyEntries => Set<IdempotencyEntry>();

    public DbSet<RiskAssessmentLog> RiskAssessmentLogs => Set<RiskAssessmentLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


