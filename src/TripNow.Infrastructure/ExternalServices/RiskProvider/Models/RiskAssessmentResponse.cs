namespace TripNow.Infrastructure.ExternalServices.RiskProvider.Models;

public sealed class RiskAssessmentResponse
{
    public int RiskScore { get; init; }
    public string Status { get; init; } = default!;
    public string? Reason { get; init; }
}


