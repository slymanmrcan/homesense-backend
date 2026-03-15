using HomeSense.Api.Data;
using HomeSense.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Models;

namespace HomeSense.Api.Services;

public class ReadingService(AppDbContext db, IAlertService alertService, IDeviceService deviceService) : IReadingService
{
    public async Task<bool> IngestAsync(IngestReadingsDto dto, CancellationToken ct = default)
    {
        bool duplicate = await db.ReadingBatches
            .AnyAsync(b => b.BatchKey == dto.BatchKey, ct);

        if (duplicate) return false;

        var device = await deviceService.ResolveDeviceAsync(dto.MacAddress, ct);

        var batch = new ReadingBatch
        {
            DeviceId = device.Id,
            BatchKey = dto.BatchKey,
            TriggeredByThreshold = dto.TriggeredByThreshold,
            TriggerSensorType = dto.TriggerSensorType,
            TriggerValue = dto.TriggerValue,
            TriggerRule = dto.TriggerRule,
            RecordedAtUtc = NormalizeUtc(dto.RecordedAtUtc),
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

    public async Task<IReadOnlyList<ReadingBatchResponseDto>> GetBatchesAsync(
        Guid deviceId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var batches = await db.ReadingBatches
            .Where(b => b.DeviceId == deviceId)
            .OrderByDescending(b => b.RecordedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.Readings)
            .AsNoTracking()
            .ToListAsync(ct);

        return batches.Select(MapBatch).ToList();
    }

    public async Task<ReadingBatchResponseDto?> GetLatestBatchAsync(
        Guid deviceId,
        CancellationToken ct = default)
    {
        var batch = await db.ReadingBatches
            .Where(b => b.DeviceId == deviceId)
            .OrderByDescending(b => b.RecordedAtUtc)
            .Include(b => b.Readings)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return batch is null ? null : MapBatch(batch);
    }

    private static ReadingBatchResponseDto MapBatch(ReadingBatch batch)
    {
        return new ReadingBatchResponseDto
        {
            Id = batch.Id,
            DeviceId = batch.DeviceId,
            BatchKey = batch.BatchKey,
            TriggeredByThreshold = batch.TriggeredByThreshold,
            TriggerSensorType = batch.TriggerSensorType,
            TriggerValue = batch.TriggerValue,
            TriggerRule = batch.TriggerRule,
            RecordedAtUtc = NormalizeUtc(batch.RecordedAtUtc),
            ReceivedAtUtc = NormalizeUtc(batch.ReceivedAtUtc),
            CreatedAtUtc = NormalizeUtc(batch.CreatedAtUtc),
            UpdatedAtUtc = NormalizeUtc(batch.UpdatedAtUtc),
            Readings = batch.Readings.Select(r => new SensorReadingResponseDto
            {
                Id = r.Id,
                BatchId = r.BatchId,
                SensorType = r.SensorType,
                Value = r.Value,
                Unit = r.Unit,
                RawPayload = r.RawPayload,
                CreatedAtUtc = NormalizeUtc(r.CreatedAtUtc),
                UpdatedAtUtc = NormalizeUtc(r.UpdatedAtUtc),
            }).ToList()
        };
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static DateTime? NormalizeUtc(DateTime? value)
    {
        return value.HasValue ? NormalizeUtc(value.Value) : null;
    }
}
