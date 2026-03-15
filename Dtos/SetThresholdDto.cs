namespace HomeSense.Api.Dtos;

public class SetThresholdDto
{
    public string SensorType { get; set; } = default!;  // "temperature"
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
}