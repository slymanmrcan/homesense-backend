using Models;

namespace HomeSense.Api.Dtos;

public class IngestReadingsDto
{
    public string MacAddress { get; set; } = default!;
    public Guid BatchKey { get; set; }
    public bool TriggeredByThreshold { get; set; }
    public string? TriggerSensorType { get; set; }
    public decimal? TriggerValue { get; set; }
    public string? TriggerRule { get; set; }
    public DateTime RecordedAtUtc { get; set; }
    public List<SensorReadingDto> Readings { get; set; } = [];
}