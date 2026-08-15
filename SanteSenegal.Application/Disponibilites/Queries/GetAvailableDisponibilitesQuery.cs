using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Disponibilites.Queries;

public record GetAvailableDisponibilitesQuery(int StructureId, int SousServiceId, DateTime Date) : IRequest<IEnumerable<Disponibilite>>;

public class GetAvailableDisponibilitesQueryHandler : IRequestHandler<GetAvailableDisponibilitesQuery, IEnumerable<Disponibilite>>
{
    private readonly IDisponibiliteRepository _repository;

    public GetAvailableDisponibilitesQueryHandler(IDisponibiliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Disponibilite>> Handle(GetAvailableDisponibilitesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAvailableAsync(request.StructureId, request.SousServiceId, request.Date, cancellationToken);
    }
}
