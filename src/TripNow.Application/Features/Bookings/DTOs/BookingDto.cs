using TripNow.Core.Enums;

namespace TripNow.Application.Features.Bookings.DTOs;

public sealed record BookingDto(
    Guid Id,
    string CustomerEmail,
    string TripCountry,
    decimal Amount,
    BookingStatus Status,
    int? RiskScore,
    string? Reason,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid CorrelationId);


