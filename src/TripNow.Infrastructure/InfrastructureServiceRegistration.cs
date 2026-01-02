using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using TripNow.Application.Common.Interfaces;
using TripNow.Core.Interfaces;
using TripNow.Core.Interfaces.Repositories;
using TripNow.Core.Interfaces.Services;
using TripNow.Infrastructure.Data;
using TripNow.Infrastructure.Data.Repositories;
using TripNow.Infrastructure.ExternalServices.RiskProvider;
using TripNow.Infrastructure.Services;

namespace TripNow.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Supabase")
                               ?? throw new InvalidOperationException("Connection string 'Supabase' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        // Risk provider HttpClient with Polly resilience policies
        var riskBaseUrl = configuration["RiskAssessment:BaseUrl"]
                         ?? throw new InvalidOperationException("RiskAssessment:BaseUrl not configured.");

        services.AddHttpClient<IRiskAssessmentService, RiskAssessmentService>(client =>
            {
                client.BaseAddress = new Uri(riskBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
            .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

        return services;
    }
}

