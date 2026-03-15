using Models;
using HomeSense.Api.Dtos;
namespace HomeSense.Api.Services;

public interface IReadingService
{
    // true: kaydedildi, false: duplicate (aynı BatchKey daha önce gelmişti)
    Task<bool> IngestAsync(IngestReadingsDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<ReadingBatchResponseDto>> GetBatchesAsync(
        Guid deviceId, int page, int pageSize, CancellationToken ct = default);
    Task<ReadingBatchResponseDto?> GetLatestBatchAsync(
        Guid deviceId, CancellationToken ct = default);
}