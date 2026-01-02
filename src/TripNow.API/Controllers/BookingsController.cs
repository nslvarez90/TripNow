using MediatR;
using Microsoft.AspNetCore.Mvc;
using TripNow.Application.Features.Bookings.Commands.CreateBooking;
using TripNow.Application.Features.Bookings.Queries.GetBookingById;

namespace TripNow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new booking with immediate response (<500ms) and PENDING_RISK_CHECK status.
    /// Risk assessment is processed asynchronously via BackgroundService.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBookingCommandResponse>> CreateBooking(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating booking for {Email} to {Country} with amount {Amount} (CorrelationId: {CorrelationId})",
            command.CustomerEmail, command.TripCountry, command.Amount, command.CorrelationId);

        var response = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(
            nameof(GetBookingById),
            new { id = response.BookingId },
            response);
    }

    /// <summary>
    /// Gets a booking by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetBookingByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetBookingByIdResponse>> GetBookingById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetBookingByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}

