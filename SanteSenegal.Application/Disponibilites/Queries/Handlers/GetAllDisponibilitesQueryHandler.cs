using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Queries.Handlers;

public class GetAllDisponibilitesQueryHandler : IRequestHandler<GetAllDisponibilitesQuery, IEnumerable<DisponibiliteDto>>
{
    private readonly IDisponibiliteRepository _repository;

    public GetAllDisponibilitesQueryHandler(IDisponibiliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DisponibiliteDto>> Handle(GetAllDisponibilitesQuery request, CancellationToken cancellationToken)
    {
        var disponibilites = await _repository.GetAllAsync();
        return disponibilites.Select(d => new DisponibiliteDto
        {
            Id = d.Id,
            Date = d.Date,
            HeureDebut = d.HeureDebut,
            HeureFin = d.HeureFin,
            EstDisponible = d.EstDisponible,
            MaxRendezVous = d.MaxRendezVous,
            NombreRendezVousPris = d.RendezVous?.Count ?? 0,
            Notes = d.Notes
        });
    }
}
