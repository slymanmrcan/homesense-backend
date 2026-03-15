using HomeSense.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeSense.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController(IAlertService alertService) : BaseController
{
    [HttpGet("{deviceId:guid}")]
    public async Task<IActionResult> GetAlerts(
        Guid deviceId,
        [FromQuery] bool onlyUnacknowledged = false,
        CancellationToken ct = default)
    {
        var alerts = await alertService.GetAlertsAsync(deviceId, onlyUnacknowledged, ct);
        return Ok(alerts);
    }

    [HttpPatch("{alertId:long}/acknowledge")]
    public async Task<IActionResult> Acknowledge(long alertId, CancellationToken ct)
    {
        await alertService.AcknowledgeAsync(alertId, ct);
        return NoContent();
    }
}