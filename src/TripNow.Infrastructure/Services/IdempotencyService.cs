using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TripNow.Application.Common.Interfaces;
using TripNow.Application.Features.Bookings.Commands.CreateBooking;
using TripNow.Infrastructure.Data;
using TripNow.Infrastructure.Data.Entities;

namespace TripNow.Infrastructure.Services;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext _context;

    public IdempotencyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse?> GetResponseIfExistsAsync<TResponse>(Guid correlationId, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var entry = await _context.IdempotencyEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.IdempotencyKey == correlationId.ToString(), cancellationToken)
            .ConfigureAwait(false);

        if (entry is null || string.IsNullOrWhiteSpace(entry.ResponseJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<TResponse>(entry.ResponseJson);
    }

    public async Task SaveResponseAsync<TResponse>(Guid correlationId, TResponse response, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(response);

        // Try to extract BookingId from response if it's a CreateBookingCommandResponse
        Guid? bookingId = null;
        if (response is CreateBookingCommandResponse createResponse)
        {
            bookingId = createResponse.BookingId;
        }
        else
        {
            // Try to extract via reflection as fallback
            var bookingIdProperty = typeof(TResponse).GetProperty("BookingId");
            if (bookingIdProperty?.PropertyType == typeof(Guid))
            {
                bookingId = (Guid?)bookingIdProperty.GetValue(response);
            }
        }

        var entry = await _context.IdempotencyEntries
            .SingleOrDefaultAsync(x => x.IdempotencyKey == correlationId.ToString(), cancellationToken)
            .ConfigureAwait(false);

        if (entry is null)
        {
            entry = new IdempotencyEntry
            {
                IdempotencyKey = correlationId.ToString(),
                BookingId = bookingId ?? Guid.Empty,
                RequestHash = string.Empty, // Puede calcularse según el cuerpo si se requiere
                ResponseJson = json,
                CreatedAt = DateTime.UtcNow
            };

            await _context.IdempotencyEntries.AddAsync(entry, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            entry.ResponseJson = json;
            if (bookingId.HasValue && entry.BookingId == Guid.Empty)
            {
                entry.BookingId = bookingId.Value;
            }
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}


