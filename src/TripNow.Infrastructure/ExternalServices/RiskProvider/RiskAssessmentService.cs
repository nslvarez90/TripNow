using System.Net.Http.Json;
using TripNow.Core.Entities;
using TripNow.Core.Enums;
using TripNow.Core.Interfaces.Services;
using TripNow.Infrastructure.Data;
using TripNow.Infrastructure.Data.Entities;
using TripNow.Infrastructure.ExternalServices.RiskProvider.Models;

namespace TripNow.Infrastructure.ExternalServices.RiskProvider;

public sealed class RiskAssessmentService : IRiskAssessmentService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;

    public RiskAssessmentService(HttpClient httpClient, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task AssessRiskAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var request = new RiskAssessmentRequest
        {
            CustomerEmail = booking.CustomerEmail,
            TripCountry = booking.TripCountry,
            Amount = booking.Amount
        };

        RiskAssessmentResponse? response = null;
        string? errorMessage = null;

        try
        {
            using var httpResponse = await _httpClient.PostAsJsonAsync("risk/assess", request, cancellationToken)
                .ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content
                    .ReadFromJsonAsync<RiskAssessmentResponse>(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                errorMessage = $"HTTP {(int)httpResponse.StatusCode} - {httpResponse.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }

        if (response is not null && string.Equals(response.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
        {
            booking.MarkRiskApproved(response.RiskScore, response.Reason);
        }
        else
        {
            // Si falla la llamada o el proveedor devuelve rechazado, marcamos como REJECTED.
            booking.MarkRiskRejected(response?.RiskScore ?? 0, response?.Reason ?? errorMessage);
        }

        var log = new RiskAssessmentLog
        {
            BookingId = booking.Id,
            ExternalRequest = System.Text.Json.JsonSerializer.Serialize(request),
            ExternalResponse = response is null ? null : System.Text.Json.JsonSerializer.Serialize(response),
            Status = booking.Status.ToString(),
            ErrorMessage = errorMessage,
            AttemptNumber = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _context.RiskAssessmentLogs.AddAsync(log, cancellationToken).ConfigureAwait(false);
    }
}


