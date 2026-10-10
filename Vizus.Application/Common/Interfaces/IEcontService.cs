using Vizus.Application.Shipping;

namespace Vizus.Application.Common.Interfaces;

public interface IEcontService
{
    Task<List<EcontCityDto>> GetCitiesAsync(string? searchTerm, CancellationToken ct);
    Task<List<EcontOfficeDto>> GetOfficesAsync(int cityId, CancellationToken ct);
}