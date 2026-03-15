using System.Text.Json.Serialization;

namespace Models;

public class Device : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string MacAddress { get; set; } = default!;   // "AA:BB:CC:DD:EE:FF"
    public string ApiKeyHash { get; set; } = default!;   // SHA-256, plain text tutmuyoruz
    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public ICollection<ReadingBatch> ReadingBatches { get; set; } = [];

    [JsonIgnore]
    public ICollection<DeviceThreshold> Thresholds { get; set; } = [];

    [JsonIgnore]
    public ICollection<Alert> Alerts { get; set; } = [];
}
