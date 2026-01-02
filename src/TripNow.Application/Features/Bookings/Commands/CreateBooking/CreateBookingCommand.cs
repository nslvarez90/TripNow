using MediatR;
using TripNow.Application.Common.Behaviors;

namespace TripNow.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    string CustomerEmail,
    string TripCountry,
    decimal Amount,
    Guid CorrelationId) : IRequest<CreateBookingCommandResponse>, ICorrelatedRequest;


