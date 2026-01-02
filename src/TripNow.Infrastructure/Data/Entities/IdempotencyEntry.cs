namespace TripNow.Infrastructure.Data.Entities;

public class IdempotencyEntry
{
    public string IdempotencyKey { get; set; } = default!;

    public Guid BookingId { get; set; }

    public string RequestHash { get; set; } = default!;

    public string? ResponseJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


