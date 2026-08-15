using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Application.DTOs.Notifications;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Notifications.Commands;

public record EnvoyerAlerteEpidemiologiqueCommand(AlerteEpidemiologiqueRequestDto Dto) : IRequest<Result<int>>;

public class EnvoyerAlerteEpidemiologiqueCommandHandler : IRequestHandler<EnvoyerAlerteEpidemiologiqueCommand, Result<int>>
{
    private readonly INotificationService _notificationService;

    public EnvoyerAlerteEpidemiologiqueCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<int>> Handle(EnvoyerAlerteEpidemiologiqueCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        int succesCount = 0;

        foreach (var telephone in dto.Telephones)
        {
            var result = await _notificationService.EnvoyerAlerteEpidemiologiqueAsync(
                telephone, dto.TitreAlerte, dto.Conseils, dto.Langue, cancellationToken);
            if (result.Success) succesCount++;
        }

        return Result<int>.Ok(succesCount);
    }
}
