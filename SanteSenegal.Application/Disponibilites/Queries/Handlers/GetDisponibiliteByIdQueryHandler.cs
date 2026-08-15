using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Queries.Handlers;

public class GetDisponibiliteByIdQueryHandler : IRequestHandler<GetDisponibiliteByIdQuery, DisponibiliteDto>
{
    private readonly IDisponibiliteRepository _repository;

    public GetDisponibiliteByIdQueryHandler(IDisponibiliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<DisponibiliteDto> Handle(GetDisponibiliteByIdQuery request, CancellationToken cancellationToken)
    {
        var disponibilite = await _repository.GetByIdAsync(request.Id);
        if (disponibilite == null)
            return null;

        return new DisponibiliteDto
        {
            Id = disponibilite.Id,
            Date = disponibilite.Date,
            HeureDebut = disponibilite.HeureDebut,
            HeureFin = disponibilite.HeureFin,
            EstDisponible = disponibilite.EstDisponible,
            MaxRendezVous = disponibilite.MaxRendezVous,
            NombreRendezVousPris = disponibilite.RendezVous?.Count ?? 0,
            Notes = disponibilite.Notes
        };
    }
}
