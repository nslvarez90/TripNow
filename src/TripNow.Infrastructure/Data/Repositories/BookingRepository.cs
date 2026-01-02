using Microsoft.EntityFrameworkCore;
using TripNow.Core.Entities;
using TripNow.Core.Interfaces.Repositories;

namespace TripNow.Infrastructure.Data.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Booking>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

using Microsoft.EntityFrameworkCore;
using TripNow.Core.Entities;
using TripNow.Core.Interfaces.Repositories;

namespace TripNow.Infrastructure.Data.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Bookings
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Booking>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}


