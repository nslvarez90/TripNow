namespace TripNow.Infrastructure.Data.Entities;

public class RiskAssessmentLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BookingId { get; set; }

    public string? ExternalRequest { get; set; }

    public string? ExternalResponse { get; set; }

    public string Status { get; set; } = default!;

    public string? ErrorMessage { get; set; }

    public int AttemptNumber { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


