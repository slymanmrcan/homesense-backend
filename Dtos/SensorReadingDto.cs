namespace HomeSense.Api.Dtos;

public class SensorReadingDto
{
    public string SensorType { get; set; } = default!;
    public decimal Value { get; set; }
    public string Unit { get; set; } = default!;
}