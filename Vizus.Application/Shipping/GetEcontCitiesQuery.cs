using MediatR;

namespace Vizus.Application.Shipping;

public record GetEcontCitiesQuery(string? SearchTerm) : IRequest<List<EcontCityDto>>;

public class GetEcontCitiesQueryHandler : IRequestHandler<GetEcontCitiesQuery, List<EcontCityDto>>
{
    private readonly Common.Interfaces.IEcontService _econtService;
    public GetEcontCitiesQueryHandler(Common.Interfaces.IEcontService econtService) => _econtService = econtService;

    public Task<List<EcontCityDto>> Handle(GetEcontCitiesQuery request, CancellationToken ct)
        => _econtService.GetCitiesAsync(request.SearchTerm, ct);
}