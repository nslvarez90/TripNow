using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TripNow.Core.Entities;
using TripNow.Core.Enums;
using TripNow.Core.Interfaces.Services;
using TripNow.Infrastructure.Data;

namespace TripNow.Infrastructure.BackgroundServices;

public sealed class RiskAssessmentBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RiskAssessmentBackgroundService> _logger;

    public RiskAssessmentBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<RiskAssessmentBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RiskAssessmentBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var riskService = scope.ServiceProvider.GetRequiredService<IRiskAssessmentService>();

                var pendingBookings = await context.Bookings
                    .Where(b => b.Status == BookingStatus.PendingRiskCheck)
                    .OrderBy(b => b.CreatedAt)
                    .Take(10)
                    .ToListAsync(stoppingToken)
                    .ConfigureAwait(false);

                if (pendingBookings.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                foreach (var booking in pendingBookings)
                {
                    await riskService.AssessRiskAsync(booking, stoppingToken).ConfigureAwait(false);
                }

                await context.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing risk assessments.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("RiskAssessmentBackgroundService stopped.");
    }
}


