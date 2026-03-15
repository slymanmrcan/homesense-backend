using HomeSense.Api.Dtos;
using Models;

namespace HomeSense.Api.Services;

public interface IDeviceService
{
    // MAC adresine göre cihazı bul, yoksa oluştur
    Task<Device> ResolveDeviceAsync(string macAddress, CancellationToken ct = default);
    Task<Result<Device>> CreateAsync(CreateDeviceDto dto, CancellationToken ct = default);
    Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Device>> GetAllAsync(CancellationToken ct = default);
    Task<Result> SetThresholdAsync(Guid deviceId, SetThresholdDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<DeviceThreshold>> GetThresholdsAsync(Guid deviceId, CancellationToken ct = default);
}
