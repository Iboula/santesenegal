using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.RendezVous.Queries.Handlers;

public class GetRendezVousByPatientQueryHandler : IRequestHandler<GetRendezVousByPatientQuery, IEnumerable<RendezVousDto>>
{
    private readonly IRendezVousRepository _rendezVousRepository;

    public GetRendezVousByPatientQueryHandler(IRendezVousRepository rendezVousRepository)
    {
        _rendezVousRepository = rendezVousRepository;
    }

    public async Task<IEnumerable<RendezVousDto>> Handle(GetRendezVousByPatientQuery request, CancellationToken cancellationToken)
    {
        var rendezVous = await _rendezVousRepository.GetByPatientIdAsync(request.PatientId, cancellationToken);
        
        return rendezVous.Select(r => new RendezVousDto
        {
            Id = r.Id,
            Date = r.Date,
            HeureDebut = r.Heure,
            HeureFin = r.Heure.Add(TimeSpan.FromHours(1)),
            Notes = r.Notes,
            Statut = r.Statut,
            PatientId = r.PatientId,
            Patient = new PatientBasicDto
            {
                Id = r.Patient.Id,
                Nom = r.Patient.Nom,
                Prenom = r.Patient.Prenom,
                Telephone = r.Patient.Telephone
            },
            StructureId = r.StructureId,
            Structure = new StructureBasicDto
            {
                Id = r.Structure.Id,
                Nom = r.Structure.Nom,
                Type = r.Structure.Type,
                Adresse = r.Structure.Adresse
            },
            SousServiceId = r.SousServiceId,
            SousService = new SousServiceBasicDto
            {
                Id = r.SousService.Id,
                Nom = r.SousService.Nom,
                Prix = r.SousService.Prix,
                Specialite = r.SousService.Specialite
            }
        });
    }
}
