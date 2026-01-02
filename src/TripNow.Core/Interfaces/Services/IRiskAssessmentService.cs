using TripNow.Core.Entities;

namespace TripNow.Core.Interfaces.Services;

public interface IRiskAssessmentService
{
    Task AssessRiskAsync(Booking booking, CancellationToken cancellationToken = default);
}


