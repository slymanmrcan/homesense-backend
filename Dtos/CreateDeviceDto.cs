namespace HomeSense.Api.Dtos;

public class CreateDeviceDto
{
    public string Name { get; set; } = default!;
    public string MacAddress { get; set; } = default!;
}
