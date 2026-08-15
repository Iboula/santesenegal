using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Notifications.Commands;

public record EnvoyerRappelRendezVousCommand(int RendezVousId, string Langue = "fr") : IRequest<Result<Notification>>;

public class EnvoyerRappelRendezVousCommandHandler : IRequestHandler<EnvoyerRappelRendezVousCommand, Result<Notification>>
{
    private readonly INotificationService _notificationService;

    public EnvoyerRappelRendezVousCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<Notification>> Handle(EnvoyerRappelRendezVousCommand request, CancellationToken cancellationToken)
    {
        return await _notificationService.EnvoyerRappelRendezVousAsync(
            request.RendezVousId, request.Langue, cancellationToken);
    }
}
