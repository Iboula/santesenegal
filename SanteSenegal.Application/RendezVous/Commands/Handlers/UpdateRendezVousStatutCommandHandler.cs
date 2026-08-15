using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Domain.Events;
using Microsoft.Extensions.Logging;

namespace SanteSenegal.Application.RendezVous.Commands.Handlers;

public class UpdateRendezVousStatutCommandHandler : IRequestHandler<UpdateRendezVousStatutCommand, bool>
{
    private readonly IRendezVousRepository _rendezVousRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UpdateRendezVousStatutCommandHandler> _logger;

    public UpdateRendezVousStatutCommandHandler(
        IRendezVousRepository rendezVousRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<UpdateRendezVousStatutCommandHandler> logger)
    {
        _rendezVousRepository = rendezVousRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateRendezVousStatutCommand request, CancellationToken cancellationToken)
    {
        var rendezVous = await _rendezVousRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rendezVous == null)
            return false;

        var ancienStatut = rendezVous.Statut;
        rendezVous.Statut = request.Statut;

        // Si le rendez-vous est annulé, déclencher l'événement d'annulation + notification SMS
        if (request.Statut == StatutRendezVous.Annule && ancienStatut != StatutRendezVous.Annule)
        {
            rendezVous.AddDomainEvent(new RendezVousAnnuleEvent(rendezVous.Id, rendezVous.Date));

            try
            {
                var message = rendezVous.LanguePreference == "wo"
                    ? $"Salaamalekum {rendezVous.Patient.Prenom}, sa rendez-vous {rendezVous.Date:dd/MM/yyyy} ci {rendezVous.Structure.Nom} dafa neex. Boo bëggee waxtu bu bees: 800 00 50 50. SanteSenegal"
                    : $"Bonjour {rendezVous.Patient.Prenom}, votre rendez-vous du {rendezVous.Date:dd/MM/yyyy} au {rendezVous.Structure.Nom} a ete annule. Pour reprogrammer: 800 00 50 50. SanteSenegal";

                await _notificationService.EnvoyerSMSAsync(
                    rendezVous.Patient.Telephone,
                    message,
                    TypeNotification.AnnulationRendezVous,
                    "RendezVous",
                    rendezVous.Id,
                    rendezVous.LanguePreference,
                    cancellationToken);
                _logger.LogInformation("Notification d'annulation envoyee pour RDV {RdvId}", rendezVous.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Echec envoi notification annulation RDV {RdvId}", rendezVous.Id);
            }
        }

        // Si confirmé, envoyer confirmation
        if (request.Statut == StatutRendezVous.Confirme && ancienStatut != StatutRendezVous.Confirme)
        {
            try
            {
                await _notificationService.EnvoyerConfirmationRendezVousAsync(rendezVous.Id, rendezVous.LanguePreference, cancellationToken);
                _logger.LogInformation("Notification de confirmation envoyee pour RDV {RdvId}", rendezVous.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Echec envoi notification confirmation RDV {RdvId}", rendezVous.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
