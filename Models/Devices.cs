namespace Models;

public class Device : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string MacAddress { get; set; } = default!;   // "AA:BB:CC:DD:EE:FF"
    public string ApiKeyHash { get; set; } = default!;   // SHA-256, plain text tutmuyoruz
    public bool IsActive { get; set; } = true;

    public ICollection<ReadingBatch> ReadingBatches { get; set; } = [];
    public ICollection<DeviceThreshold> Thresholds { get; set; } = [];
    public ICollection<Alert> Alerts { get; set; } = [];
}