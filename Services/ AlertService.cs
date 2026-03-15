using HomeSense.Api.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace HomeSense.Api.Services;

public class AlertService(AppDbContext db) : IAlertService
{
    public async Task EvaluateAsync(ReadingBatch batch, CancellationToken ct = default)
    {
        var thresholds = await db.DeviceThresholds
            .Where(t => t.DeviceId == batch.DeviceId && t.IsActive)
            .AsNoTracking()
            .ToListAsync(ct);

        if (thresholds.Count == 0) return;

        var alerts = new List<Alert>();

        foreach (var reading in batch.Readings)
        {
            var threshold = thresholds
                .FirstOrDefault(t => t.SensorType == reading.SensorType);

            if (threshold is null) continue;

            if (threshold.MinValue.HasValue && reading.Value < threshold.MinValue.Value)
            {
                alerts.Add(new Alert
                {
                    DeviceId       = batch.DeviceId,
                    BatchId        = batch.Id,
                    SensorType     = reading.SensorType,
                    TriggerValue   = reading.Value,
                    ThresholdValue = threshold.MinValue.Value,
                    Rule           = $"< {threshold.MinValue.Value}"
                });
            }

            if (threshold.MaxValue.HasValue && reading.Value > threshold.MaxValue.Value)
            {
                alerts.Add(new Alert
                {
                    DeviceId       = batch.DeviceId,
                    BatchId        = batch.Id,
                    SensorType     = reading.SensorType,
                    TriggerValue   = reading.Value,
                    ThresholdValue = threshold.MaxValue.Value,
                    Rule           = $"> {threshold.MaxValue.Value}"
                });
            }
        }

        if (alerts.Count == 0) return;

        db.Alerts.AddRange(alerts);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Alert>> GetAlertsAsync(
        Guid deviceId, bool onlyUnacknowledged = false, CancellationToken ct = default)
    {
        var query = db.Alerts.Where(a => a.DeviceId == deviceId);

        if (onlyUnacknowledged)
            query = query.Where(a => !a.IsAcknowledged);

        return await query
            .OrderByDescending(a => a.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AcknowledgeAsync(long alertId, CancellationToken ct = default)
    {
        await db.Alerts
            .Where(a => a.Id == alertId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.IsAcknowledged, true)
                .SetProperty(a => a.UpdatedAtUtc, DateTime.UtcNow), ct);
    }
}