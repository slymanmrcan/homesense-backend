using HomeSense.Api.Data;
using HomeSense.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Models;

namespace HomeSense.Api.Services;

public class DeviceService(AppDbContext db) : IDeviceService
{
    public async Task<Device> ResolveDeviceAsync(string macAddress, CancellationToken ct = default)
    {
        macAddress = NormalizeMacAddress(macAddress);

        var device = await db.Devices
            .FirstOrDefaultAsync(d => d.MacAddress == macAddress, ct);

        if (device is not null) return device;

        // İlk kez görülen cihaz — otomatik kaydet
        device = new Device
        {
            Id         = Guid.NewGuid(),
            MacAddress = macAddress,
            Name       = $"Device {macAddress}",  // dashboard'dan sonra düzenlenebilir
            ApiKeyHash = string.Empty,             // middleware zaten doğruladı
            IsActive   = true
        };

        db.Devices.Add(device);
        await db.SaveChangesAsync(ct);

        return device;
    }

    public async Task<Result<Device>> CreateAsync(CreateDeviceDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<Device>.Fail(Error.Validation("name is required."));

        if (string.IsNullOrWhiteSpace(dto.MacAddress))
            return Result<Device>.Fail(Error.Validation("macAddress is required."));

        var normalizedMacAddress = NormalizeMacAddress(dto.MacAddress);

        var exists = await db.Devices
            .AnyAsync(d => d.MacAddress == normalizedMacAddress, ct);

        if (exists)
            return Result<Device>.Fail(Error.Conflict($"Device with macAddress '{normalizedMacAddress}' already exists."));

        var device = new Device
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            MacAddress = normalizedMacAddress,
            ApiKeyHash = string.Empty,
            IsActive = true
        };

        db.Devices.Add(device);
        await db.SaveChangesAsync(ct);

        return Result<Device>.Success(device);
    }

    public async Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<IReadOnlyList<Device>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Devices
            .Where(d => d.IsActive)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Result> SetThresholdAsync(Guid deviceId, SetThresholdDto dto, CancellationToken ct = default)
    {
        var deviceExists = await db.Devices
            .AnyAsync(d => d.Id == deviceId && d.IsActive, ct);

        if (!deviceExists)
            return Result.Fail(Error.NotFound($"Device '{deviceId}' was not found."));

        if (dto.MinValue.HasValue && dto.MaxValue.HasValue && dto.MinValue > dto.MaxValue)
            return Result.Fail(Error.Validation("minValue cannot be greater than maxValue."));

        var existing = await db.DeviceThresholds
            .FirstOrDefaultAsync(t => t.DeviceId == deviceId && t.SensorType == dto.SensorType, ct);

        if (existing is null)
        {
            db.DeviceThresholds.Add(new DeviceThreshold
            {
                DeviceId = deviceId,
                SensorType = dto.SensorType,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue
            });
        }
        else
        {
            existing.MinValue = dto.MinValue;
            existing.MaxValue = dto.MaxValue;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<IReadOnlyList<DeviceThreshold>> GetThresholdsAsync(
        Guid deviceId, CancellationToken ct = default)
    {
        return await db.DeviceThresholds
            .Where(t => t.DeviceId == deviceId && t.IsActive)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    private static string NormalizeMacAddress(string macAddress)
    {
        return macAddress.Trim().Replace("-", ":").ToUpperInvariant();
    }
}
