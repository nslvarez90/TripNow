namespace TripNow.Infrastructure.ExternalServices.RiskProvider.Models;

public sealed class RiskAssessmentRequest
{
    public string CustomerEmail { get; init; } = default!;
    public string TripCountry { get; init; } = default!;
    public decimal Amount { get; init; }
}


