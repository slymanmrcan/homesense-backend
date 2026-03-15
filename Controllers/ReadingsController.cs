using HomeSense.Api.Services;
using HomeSense.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HomeSense.Api.Controllers;

[ApiController]
[Route("api/readings")]
public class ReadingsController(IReadingService readingService) : BaseController
{
    [HttpPost]
    [EnableRateLimiting("ingest")]
    public async Task<IActionResult> Ingest(
        [FromBody] IngestReadingsDto dto,
        CancellationToken ct)
    {
        var created = await readingService.IngestAsync(dto, ct);

        // Duplicate batch — idempotent, yine 200 dön
        if (!created) return Ok(new { message = "Duplicate batch, ignored." });

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{deviceId:guid}/latest")]
    public async Task<IActionResult> GetLatest(Guid deviceId, CancellationToken ct)
    {
        var batch = await readingService.GetLatestBatchAsync(deviceId, ct);
        if (batch is null) return NotFound();
        return Ok(batch);
    }

    [HttpGet("{deviceId:guid}")]
    public async Task<IActionResult> GetHistory(
        Guid deviceId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var batches = await readingService.GetBatchesAsync(deviceId, page, pageSize, ct);
        return Ok(batches);
    }
}