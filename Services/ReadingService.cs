using HomeSense.Api.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using HomeSense.Api.Dtos;
namespace HomeSense.Api.Services;

public class ReadingService(AppDbContext db, IAlertService alertService, IDeviceService deviceService) : IReadingService
{
    public async Task<bool> IngestAsync(IngestReadingsDto dto, CancellationToken ct = default)
    {
        bool duplicate = await db.ReadingBatches
            .AnyAsync(b => b.BatchKey == dto.BatchKey, ct);

        if (duplicate) return false;

        // MAC → DeviceId
        var device = await deviceService.ResolveDeviceAsync(dto.MacAddress, ct);

        var batch = new ReadingBatch
        {
            DeviceId = device.Id,  // artık burası dolu
            BatchKey = dto.BatchKey,
            TriggeredByThreshold = dto.TriggeredByThreshold,
            TriggerSensorType = dto.TriggerSensorType,
            TriggerValue = dto.TriggerValue,
            TriggerRule = dto.TriggerRule,
            RecordedAtUtc = dto.RecordedAtUtc,
            ReceivedAtUtc = DateTime.UtcNow,
            Readings = dto.Readings.Select(r => new SensorReading
            {
                SensorType = r.SensorType,
                Value = r.Value,
                Unit = r.Unit,
            }).ToList()
        };

        db.ReadingBatches.Add(batch);
        await db.SaveChangesAsync(ct);

        await alertService.EvaluateAsync(batch, ct);

        return true;
    }

    public async Task<IReadOnlyList<ReadingBatch>> GetBatchesAsync(
        Guid deviceId, int page, int pageSize, CancellationToken ct = default)
    {
        return await db.ReadingBatches
            .Where(b => b.DeviceId == deviceId)
            .OrderByDescending(b => b.RecordedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.Readings)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<ReadingBatch?> GetLatestBatchAsync(Guid deviceId, CancellationToken ct = default)
    {
        return await db.ReadingBatches
            .Where(b => b.DeviceId == deviceId)
            .OrderByDescending(b => b.RecordedAtUtc)
            .Include(b => b.Readings)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }
}