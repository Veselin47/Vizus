using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Vizus.Application.Common.Interfaces;
using Vizus.Application.Shipping;

namespace Vizus.Infrastructure.Shipping;

public class EcontService : IEcontService
{
    private const string CitiesCacheKey = "econt-cities-bgr";
    private const int MaxResults = 15;

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public EcontService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://ee.econt.com/services/");
        _cache = cache;
    }

    // Списъкът с градове се сваля веднъж и се пази 24 часа, защото рядко се променя
    private async Task<List<EcontCityDto>> GetAllCitiesAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(CitiesCacheKey, out List<EcontCityDto>? cached) && cached is not null)
            return cached;

        var response = await _httpClient.PostAsJsonAsync(
            "Nomenclatures/NomenclaturesService.getCities.json",
            new { countryCode = "BGR" },
            ct);
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
                    Name = city.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
                    NameEn = city.TryGetProperty("nameEn", out var nameEn) ? nameEn.GetString() ?? "" : "",
                    PostCode = city.TryGetProperty("postCode", out var post) ? post.GetString() ?? "" : ""
                });
            }
        }

        _cache.Set(CitiesCacheKey, cities, TimeSpan.FromHours(24));
        return cities;
    }

    public async Task<List<EcontCityDto>> GetCitiesAsync(string? searchTerm, CancellationToken ct)
    {
        var all = await GetAllCitiesAsync(ct);
        var term = searchTerm?.Trim();

        if (string.IsNullOrEmpty(term))
            return all.Take(MaxResults).ToList();

        return all
            .Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                     || c.NameEn.Contains(term, StringComparison.OrdinalIgnoreCase))
            // градовете, които започват с въведеното, излизат първи ("Ве" -> Велико Търново преди Дебелец)
            .OrderByDescending(c => c.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase)
                                 || c.NameEn.StartsWith(term, StringComparison.OrdinalIgnoreCase))
            .ThenBy(c => c.Name)
            .Take(MaxResults)
            .ToList();
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