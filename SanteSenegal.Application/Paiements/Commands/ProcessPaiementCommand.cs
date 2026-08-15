using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Application.DTOs.Paiements;
using SanteSenegal.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SanteSenegal.Application.Paiements.Commands;

public record ProcessPaiementCommand(PaiementCreateDto Dto) : IRequest<Result<Paiement>>;

public class ProcessPaiementCommandHandler : IRequestHandler<ProcessPaiementCommand, Result<Paiement>>
{
    private readonly IPaiementService _paiementService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ProcessPaiementCommandHandler> _logger;

    public ProcessPaiementCommandHandler(
        IPaiementService paiementService,
        INotificationService notificationService,
        ILogger<ProcessPaiementCommandHandler> logger)
    {
        _paiementService = paiementService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<Paiement>> Handle(ProcessPaiementCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var result = await _paiementService.InitierPaiementMobileAsync(
            dto.RendezVousId,
            dto.Montant,
            dto.ModePaiement,
            dto.NumeroTelephone,
            dto.Description);

        if (result.Success)
        {
            // Envoyer confirmation SMS de paiement initié
            try
            {
                await _notificationService.EnvoyerConfirmationPaiementAsync(result.Value!.Id, "fr", cancellationToken);
                _logger.LogInformation("Notification de paiement envoyee pour Paiement {PaiementId}", result.Value.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Echec envoi notification paiement {PaiementId}", result.Value!.Id);
            }
        }

        return result;
    }
}
