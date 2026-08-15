using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.RendezVous.Queries.Handlers;

public class GetRendezVousByIdQueryHandler : IRequestHandler<GetRendezVousByIdQuery, RendezVousDto?>
{
    private readonly IRendezVousRepository _rendezVousRepository;

    public GetRendezVousByIdQueryHandler(IRendezVousRepository rendezVousRepository)
    {
        _rendezVousRepository = rendezVousRepository;
    }

    public async Task<RendezVousDto?> Handle(GetRendezVousByIdQuery request, CancellationToken cancellationToken)
    {
        var rendezVous = await _rendezVousRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rendezVous == null) return null;

        return new RendezVousDto
        {
            Id = rendezVous.Id,
            Date = rendezVous.Date,
            HeureDebut = rendezVous.Heure,
            HeureFin = rendezVous.Heure.Add(TimeSpan.FromHours(1)),
            Notes = rendezVous.Notes,
            Statut = rendezVous.Statut,
            PatientId = rendezVous.PatientId,
            Patient = new PatientBasicDto
            {
                Id = rendezVous.Patient.Id,
                Nom = rendezVous.Patient.Nom,
                Prenom = rendezVous.Patient.Prenom,
                Telephone = rendezVous.Patient.Telephone
            },
            StructureId = rendezVous.StructureId,
            Structure = new StructureBasicDto
            {
                Id = rendezVous.Structure.Id,
                Nom = rendezVous.Structure.Nom,
                Type = rendezVous.Structure.Type,
                Adresse = rendezVous.Structure.Adresse
            },
            SousServiceId = rendezVous.SousServiceId,
            SousService = new SousServiceBasicDto
            {
                Id = rendezVous.SousService.Id,
                Nom = rendezVous.SousService.Nom,
                Prix = rendezVous.SousService.Prix,
                Specialite = rendezVous.SousService.Specialite
            }
        };
    }
}
