using TripNow.Core.Enums;

namespace TripNow.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommandResponse(
    Guid BookingId,
    BookingStatus Status,
    Guid CorrelationId);


