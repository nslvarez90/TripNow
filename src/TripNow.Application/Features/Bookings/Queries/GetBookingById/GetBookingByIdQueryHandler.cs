using AutoMapper;
using MediatR;
using TripNow.Application.Features.Bookings.DTOs;
using TripNow.Core.Interfaces.Repositories;

namespace TripNow.Application.Features.Bookings.Queries.GetBookingById;

public sealed class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, GetBookingByIdResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetBookingByIdQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<GetBookingByIdResponse> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        var dto = booking is null ? null : _mapper.Map<BookingDto>(booking);

        return new GetBookingByIdResponse(dto);
    }
}


