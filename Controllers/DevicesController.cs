using HomeSense.Api.Dtos;
using HomeSense.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace HomeSense.Api.Controllers;

[ApiController]
[Route("api/devices")]
public class DevicesController(IDeviceService deviceService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto, CancellationToken ct)
    {
        var result = await deviceService.CreateAsync(dto, ct);
        if (!result.IsSuccess) return FromError(result.Error!);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var devices = await deviceService.GetAllAsync(ct);
        return Ok(devices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var device = await deviceService.GetByIdAsync(id, ct);
        if (device is null) return NotFound();
        return Ok(device);
    }
    [HttpPost("{deviceId:guid}/thresholds")]
    public async Task<IActionResult> SetThreshold(
    Guid deviceId,
    [FromBody] SetThresholdDto dto,
    CancellationToken ct)
    {
        var result = await deviceService.SetThresholdAsync(deviceId, dto, ct);
        if (!result.IsSuccess) return FromError(result.Error!);
        return NoContent();
    }

    [HttpGet("{deviceId:guid}/thresholds")]
    public async Task<IActionResult> GetThresholds(Guid deviceId, CancellationToken ct)
    {
        var thresholds = await deviceService.GetThresholdsAsync(deviceId, ct);
        return Ok(thresholds);
    }
}
