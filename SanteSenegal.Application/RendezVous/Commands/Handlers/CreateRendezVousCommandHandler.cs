using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SanteSenegal.Application.RendezVous.Commands.Handlers;

public class CreateRendezVousCommandHandler : IRequestHandler<CreateRendezVousCommand, RendezVousDto>
{
    private readonly IRendezVousRepository _rendezVousRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateRendezVousCommandHandler> _logger;

    public CreateRendezVousCommandHandler(
        IRendezVousRepository rendezVousRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<CreateRendezVousCommandHandler> logger)
    {
        _rendezVousRepository = rendezVousRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<RendezVousDto> Handle(CreateRendezVousCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RendezVous;
        var rendezVous = new Domain.Entities.RendezVous
        {
            Date = dto.Date,
            Heure = dto.Heure,
            Motif = dto.Motif,
            Notes = dto.Notes,
            Statut = StatutRendezVous.EnAttente,
            PatientId = dto.PatientId,
            StructureId = dto.StructureId,
            ServiceId = dto.ServiceId,
            SousServiceId = dto.SousServiceId,
            DisponibiliteId = dto.DisponibiliteId,
            LanguePreference = "fr"
        };

        await _rendezVousRepository.AddAsync(rendezVous, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ── Notification SMS de confirmation ──
        try
        {
            await _notificationService.EnvoyerConfirmationRendezVousAsync(rendezVous.Id, rendezVous.LanguePreference, cancellationToken);
            _logger.LogInformation("Notification de confirmation envoyee pour RDV {RdvId}", rendezVous.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Echec envoi notification confirmation RDV {RdvId}", rendezVous.Id);
        }

        // Recharger le rendez-vous avec toutes les relations
        var rendezVousComplet = await _rendezVousRepository.GetByIdAsync(rendezVous.Id, cancellationToken);

        return new RendezVousDto
        {
            Id = rendezVousComplet!.Id,
            Date = rendezVousComplet.Date,
            HeureDebut = rendezVousComplet.Heure,
            HeureFin = rendezVousComplet.Heure.Add(TimeSpan.FromHours(1)), // Estimation
            Notes = rendezVousComplet.Notes,
            Statut = rendezVousComplet.Statut,
            PatientId = rendezVousComplet.PatientId,
            Patient = new PatientBasicDto
            {
                Id = rendezVousComplet.Patient.Id,
                Nom = rendezVousComplet.Patient.Nom,
                Prenom = rendezVousComplet.Patient.Prenom,
                Telephone = rendezVousComplet.Patient.Telephone
            },
            StructureId = rendezVousComplet.StructureId,
            Structure = new StructureBasicDto
            {
                Id = rendezVousComplet.Structure.Id,
                Nom = rendezVousComplet.Structure.Nom,
                Type = rendezVousComplet.Structure.Type,
                Adresse = rendezVousComplet.Structure.Adresse
            },
            SousServiceId = rendezVousComplet.SousServiceId,
            SousService = new SousServiceBasicDto
            {
                Id = rendezVousComplet.SousService.Id,
                Nom = rendezVousComplet.SousService.Nom,
                Prix = rendezVousComplet.SousService.Prix,
                Specialite = rendezVousComplet.SousService.Specialite
            }
        };
    }
}
