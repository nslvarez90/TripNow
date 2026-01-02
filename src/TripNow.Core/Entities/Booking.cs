using TripNow.Core.Enums;

namespace TripNow.Core.Entities;

public class Booking
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string CustomerEmail { get; private set; } = default!;

    public string TripCountry { get; private set; } = default!;

    public decimal Amount { get; private set; }

    public BookingStatus Status { get; private set; } = BookingStatus.PendingRiskCheck;

    public int? RiskScore { get; private set; }

    public string? Reason { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public Guid CorrelationId { get; private set; }

    private Booking()
    {
        // Para EF Core
    }

    public Booking(
        string customerEmail,
        string tripCountry,
        decimal amount,
        Guid correlationId)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required.", nameof(customerEmail));

        if (string.IsNullOrWhiteSpace(tripCountry))
            throw new ArgumentException("Trip country is required.", nameof(tripCountry));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        CustomerEmail = customerEmail;
        TripCountry = tripCountry;
        Amount = amount;
        CorrelationId = correlationId;
    }

    public void MarkRiskApproved(int riskScore, string? reason = null)
    {
        Status = BookingStatus.Approved;
        RiskScore = riskScore;
        Reason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkRiskRejected(int riskScore, string? reason)
    {
        Status = BookingStatus.Rejected;
        RiskScore = riskScore;
        Reason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}


