using MediatR;

namespace Vizus.Application.Shipping;

public record GetEcontOfficesQuery(int CityId) : IRequest<List<EcontOfficeDto>>;

public class GetEcontOfficesQueryHandler : IRequestHandler<GetEcontOfficesQuery, List<EcontOfficeDto>>
{
    private readonly Common.Interfaces.IEcontService _econtService;
    public GetEcontOfficesQueryHandler(Common.Interfaces.IEcontService econtService) => _econtService = econtService;

    public Task<List<EcontOfficeDto>> Handle(GetEcontOfficesQuery request, CancellationToken ct)
        => _econtService.GetOfficesAsync(request.CityId, ct);
}