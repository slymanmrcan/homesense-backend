namespace Models;

public class SensorReading : BaseEntity
{
    public long Id { get; set; }

    public long BatchId { get; set; }
    public ReadingBatch Batch { get; set; } = default!;

    public string SensorType { get; set; } = default!;  // "temperature" | "humidity" | "distance" | "vibration" | "light"
    public decimal Value { get; set; }
    public string Unit { get; set; } = default!;         // "C" | "%" | "cm" | "lux" | "bool"

    // Vibration gibi bool sensörler için — 0/1 olarak Value'ya yazılır, bu alan opsiyonel açıklama
    public string? RawPayload { get; set; }
}