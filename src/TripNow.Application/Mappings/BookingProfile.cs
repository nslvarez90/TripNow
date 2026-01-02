using AutoMapper;
using TripNow.Application.Features.Bookings.DTOs;
using TripNow.Core.Entities;

namespace TripNow.Application.Mappings;

public sealed class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingDto>();
    }
}


