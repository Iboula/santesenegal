using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Notifications.Queries;

public record GetNotificationsQuery : IRequest<List<Notification>>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<Notification>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Notification>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
