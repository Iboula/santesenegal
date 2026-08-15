using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Notifications.Queries;

public record GetNotificationsByStatutQuery(StatutNotification Statut) : IRequest<List<Notification>>;

public class GetNotificationsByStatutQueryHandler : IRequestHandler<GetNotificationsByStatutQuery, List<Notification>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsByStatutQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Notification>> Handle(GetNotificationsByStatutQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByStatutAsync(request.Statut, cancellationToken);
    }
}
