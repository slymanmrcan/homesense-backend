using System.Text.Json.Serialization;

namespace Models;

public class DeviceThreshold : BaseEntity
{
    public int Id { get; set; }

    public Guid DeviceId { get; set; }

    [JsonIgnore]
    public Device Device { get; set; } = default!;

    public string SensorType { get; set; } = default!;  // "temperature"

    public decimal? MinValue { get; set; }   // null = alt sınır yok
    public decimal? MaxValue { get; set; }   // null = üst sınır yok

    public bool IsActive { get; set; } = true;
}
