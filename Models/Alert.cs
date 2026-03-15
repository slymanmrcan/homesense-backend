using System.Text.Json.Serialization;

namespace Models;

public class Alert : BaseEntity
{
    public long Id { get; set; }

    public Guid DeviceId { get; set; }

    [JsonIgnore]
    public Device Device { get; set; } = default!;

    public long BatchId { get; set; }

    [JsonIgnore]
    public ReadingBatch Batch { get; set; } = default!;

    public string SensorType { get; set; } = default!;  // "temperature"
    public decimal TriggerValue { get; set; }            // 9.8  — o anki ölçüm
    public decimal ThresholdValue { get; set; }          // 10.0 — eşik değeri
    public string Rule { get; set; } = default!;         // "< 10" 
    public bool IsAcknowledged { get; set; }             // dashboard'dan okundu mu
}
