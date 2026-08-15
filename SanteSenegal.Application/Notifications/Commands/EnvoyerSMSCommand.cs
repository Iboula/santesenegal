using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Application.DTOs.Notifications;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Notifications.Commands;

public record EnvoyerSMSCommand(EnvoyerSMSRequestDto Dto) : IRequest<Result<Notification>>;

public class EnvoyerSMSCommandHandler : IRequestHandler<EnvoyerSMSCommand, Result<Notification>>
{
    private readonly INotificationService _notificationService;

    public EnvoyerSMSCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<Notification>> Handle(EnvoyerSMSCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        return await _notificationService.EnvoyerSMSAsync(
            dto.Telephone,
            dto.Message,
            dto.Type,
            dto.ReferenceType,
            dto.ReferenceId,
            dto.Langue,
            cancellationToken);
    }
}
