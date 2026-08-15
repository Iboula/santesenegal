using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Abstractions;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<Notification>> GetPendingAsync(CancellationToken ct = default);
    Task<List<Notification>> GetByReferenceAsync(string referenceType, int referenceId, CancellationToken ct = default);
    Task<List<Notification>> GetByTelephoneAsync(string telephone, CancellationToken ct = default);
    Task<List<Notification>> GetByStatutAsync(StatutNotification statut, CancellationToken ct = default);
}
