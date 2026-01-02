using AutoMapper;
using MediatR;
using TripNow.Application.Features.Bookings.DTOs;
using TripNow.Core.Entities;
using TripNow.Core.Enums;
using TripNow.Core.Interfaces;
using TripNow.Core.Interfaces.Repositories;

namespace TripNow.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingCommandResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreateBookingCommandResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new Booking(
            request.CustomerEmail,
            request.TripCountry,
            request.Amount,
            request.CorrelationId);

        await _bookingRepository.AddAsync(booking, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Response must be serializable for idempotency storage.
        return new CreateBookingCommandResponse(
            booking.Id,
            BookingStatus.PendingRiskCheck,
            booking.CorrelationId);
    }
}


