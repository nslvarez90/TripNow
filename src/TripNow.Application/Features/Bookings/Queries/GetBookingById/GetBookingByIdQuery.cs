using MediatR;
using TripNow.Application.Features.Bookings.DTOs;

namespace TripNow.Application.Features.Bookings.Queries.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<GetBookingByIdResponse>;


