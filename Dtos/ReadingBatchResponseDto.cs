namespace HomeSense.Api.Dtos;

public class ReadingBatchResponseDto
{
    public long Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid BatchKey { get; set; }
    public bool TriggeredByThreshold { get; set; }
    public string? TriggerSensorType { get; set; }
    public decimal? TriggerValue { get; set; }
    public string? TriggerRule { get; set; }
    public DateTime RecordedAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public List<SensorReadingResponseDto> Readings { get; set; } = [];
}

public class SensorReadingResponseDto
{
    public long Id { get; set; }
    public long BatchId { get; set; }
    public string SensorType { get; set; } = default!;
    public decimal Value { get; set; }
    public string Unit { get; set; } = default!;
    public string? RawPayload { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
