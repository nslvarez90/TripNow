namespace TripNow.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<TResponse?> GetResponseIfExistsAsync<TResponse>(Guid correlationId, CancellationToken cancellationToken = default)
        where TResponse : class;

    Task SaveResponseAsync<TResponse>(Guid correlationId, TResponse response, CancellationToken cancellationToken = default)
        where TResponse : class;
}


