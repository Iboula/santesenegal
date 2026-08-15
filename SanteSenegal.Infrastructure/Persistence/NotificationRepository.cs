using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Infrastructure.Persistence;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly SanteDbContext _context;

    public NotificationRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetPendingAsync(CancellationToken ct = default)
    {
        return await _context.Notifications
            .Where(n => n.Statut == StatutNotification.EnAttente
                     && (n.DatePlanifie == null || n.DatePlanifie <= DateTime.UtcNow))
            .OrderBy(n => n.DatePlanifie ?? n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Notification>> GetByReferenceAsync(string referenceType, int referenceId, CancellationToken ct = default)
    {
        return await _context.Notifications
            .Where(n => n.ReferenceType == referenceType && n.ReferenceId == referenceId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Notification>> GetByTelephoneAsync(string telephone, CancellationToken ct = default)
    {
        return await _context.Notifications
            .Where(n => n.DestinataireTelephone == telephone)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Notification>> GetByStatutAsync(StatutNotification statut, CancellationToken ct = default)
    {
        return await _context.Notifications
            .Where(n => n.Statut == statut)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }
}
