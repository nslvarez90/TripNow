using MediatR;
using TripNow.Application.Common.Interfaces;

namespace TripNow.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior to ensure idempotent command handling based on CorrelationId.
/// </summary>
public sealed class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICorrelatedRequest
    where TResponse : class
{
    private readonly IIdempotencyService _idempotencyService;

    public IdempotencyBehavior(IIdempotencyService idempotencyService)
    {
        _idempotencyService = idempotencyService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var existing = await _idempotencyService
            .GetResponseIfExistsAsync<TResponse>(request.CorrelationId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is not null)
        {
            return existing;
        }

        var response = await next().ConfigureAwait(false);

        if (response is not null)
        {
            await _idempotencyService
                .SaveResponseAsync(request.CorrelationId, response, cancellationToken)
                .ConfigureAwait(false);
        }

        return response;
    }
}

public interface ICorrelatedRequest
{
    Guid CorrelationId { get; }
}


