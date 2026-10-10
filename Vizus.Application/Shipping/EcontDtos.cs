namespace Vizus.Application.Shipping;

public class EcontCityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
}

public class EcontOfficeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}