namespace Models;

public class ReadingBatch : BaseEntity
{
    public long Id { get; set; }

    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public Guid BatchKey { get; set; }

    public bool TriggeredByThreshold { get; set; }

    // Hangi sensör tetikledi — periyodik ise null
    public string? TriggerSensorType { get; set; }  // "temperature"
    public decimal? TriggerValue { get; set; }       // 9.8
    public string? TriggerRule { get; set; }         // "< 10"

    public DateTime RecordedAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; }

    public ICollection<SensorReading> Readings { get; set; } = [];
}