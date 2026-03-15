using Models;
namespace HomeSense.Api.Services;

public interface IAlertService
{
    Task EvaluateAsync(ReadingBatch batch, CancellationToken ct = default);
    Task<IReadOnlyList<Alert>> GetAlertsAsync(Guid deviceId, bool onlyUnacknowledged = false, CancellationToken ct = default);
    Task AcknowledgeAsync(long alertId, CancellationToken ct = default);
}