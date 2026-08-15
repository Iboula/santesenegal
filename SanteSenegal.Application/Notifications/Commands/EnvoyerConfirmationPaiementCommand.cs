using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Notifications.Commands;

public record EnvoyerConfirmationPaiementCommand(int PaiementId, string Langue = "fr") : IRequest<Result<Notification>>;

public class EnvoyerConfirmationPaiementCommandHandler : IRequestHandler<EnvoyerConfirmationPaiementCommand, Result<Notification>>
{
    private readonly INotificationService _notificationService;

    public EnvoyerConfirmationPaiementCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<Notification>> Handle(EnvoyerConfirmationPaiementCommand request, CancellationToken cancellationToken)
    {
        return await _notificationService.EnvoyerConfirmationPaiementAsync(
            request.PaiementId, request.Langue, cancellationToken);
    }
}
