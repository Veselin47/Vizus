using System.Net.Http.Json;
using System.Text.Json;
using Vizus.Application.Common.Interfaces;
using Vizus.Application.Shipping;

namespace Vizus.Infrastructure.Shipping;

public class EcontService : IEcontService
{
    private readonly HttpClient _httpClient;

    public EcontService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://ee.econt.com/services/");
    }

    public async Task<List<EcontCityDto>> GetCitiesAsync(string? searchTerm, CancellationToken ct)
    {
        var requestBody = new
        {
            countryCode = "BGR",
            nameText = searchTerm ?? ""
        };

        var response = await _httpClient.PostAsJsonAsync("Nomenclatures/NomenclaturesService.getCities.json", requestBody, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        var cities = new List<EcontCityDto>();

        if (json.TryGetProperty("cities", out var citiesArray))
        {
            foreach (var city in citiesArray.EnumerateArray())
            {
                cities.Add(new EcontCityDto
                {
                    Id = city.GetProperty("id").GetInt32(),
                    Name = city.GetProperty("name").GetString() ?? "",
                    PostCode = city.GetProperty("postCode").GetString() ?? ""
                });
            }
        }

        return cities;
    }

    public async Task<List<EcontOfficeDto>> GetOfficesAsync(int cityId, CancellationToken ct)
    {
        var requestBody = new { cityID = cityId };

        var response = await _httpClient.PostAsJsonAsync("Nomenclatures/NomenclaturesService.getOffices.json", requestBody, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        var offices = new List<EcontOfficeDto>();

        if (json.TryGetProperty("offices", out var officesArray))
        {
            foreach (var office in officesArray.EnumerateArray())
            {
                offices.Add(new EcontOfficeDto
                {
                    Code = office.GetProperty("code").GetString() ?? "",
                    Name = office.GetProperty("name").GetString() ?? "",
                    Address = office.TryGetProperty("address", out var addr) && addr.TryGetProperty("fullAddress", out var fullAddr)
                        ? fullAddr.GetString() ?? ""
                        : ""
                });
            }
        }

        return offices;
    }
}