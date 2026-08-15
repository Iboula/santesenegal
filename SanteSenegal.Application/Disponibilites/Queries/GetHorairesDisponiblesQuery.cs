using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Queries;

public record GetHorairesDisponiblesQuery(int StructureId, int SousServiceId, DateTime Date) : IRequest<IEnumerable<TimeSpan>>;

public class GetHorairesDisponiblesQueryHandler : IRequestHandler<GetHorairesDisponiblesQuery, IEnumerable<TimeSpan>>
{
    private readonly IDisponibiliteRepository _repository;

    public GetHorairesDisponiblesQueryHandler(IDisponibiliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TimeSpan>> Handle(GetHorairesDisponiblesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHorairesDisponiblesAsync(request.StructureId, request.SousServiceId, request.Date, cancellationToken);
    }
}
