using TripNow.Core.Entities;

namespace TripNow.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}


