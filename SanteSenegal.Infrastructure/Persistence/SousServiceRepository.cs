using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence;

public class SousServiceRepository : BaseRepository<SousService>, ISousServiceRepository
{
    private readonly SanteDbContext _context;

    public SousServiceRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SousService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SousService>()
            .Where(s => s.ServiceId == serviceId && s.EstDisponible)
            .Include(s => s.Disponibilites)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SousService>> GetBySpecialiteAsync(string specialite, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SousService>()
            .Where(s => s.Specialite == specialite && s.EstDisponible)
            .Include(s => s.Service)
            .Include(s => s.Disponibilites)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SousService>> GetAvailableAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SousService>()
            .Where(s => s.ServiceId == serviceId && 
                s.EstDisponible && 
                s.Disponibilites.Any(d => d.Date.Date == date.Date && d.EstDisponible))
            .Include(s => s.Disponibilites)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SousService>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SousService>()
            .Where(s => s.EstDisponible &&
                (s.Nom.Contains(searchTerm) ||
                s.Description.Contains(searchTerm) ||
                s.Specialite!.Contains(searchTerm)))
            .Include(s => s.Service)
            .Include(s => s.Disponibilites)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetPrixAsync(int sousServiceId, CancellationToken cancellationToken = default)
    {
        var sousService = await _context.Set<SousService>()
            .FirstOrDefaultAsync(s => s.Id == sousServiceId, cancellationToken);
        
        return sousService?.Prix ?? 0;
    }

    public async Task<bool> UpdatePrixAsync(int sousServiceId, decimal nouveauPrix, CancellationToken cancellationToken = default)
    {
        var sousService = await _context.Set<SousService>()
            .FirstOrDefaultAsync(s => s.Id == sousServiceId, cancellationToken);

        if (sousService == null)
            return false;

        sousService.Prix = nouveauPrix;
        return true;
    }

    public async Task<IEnumerable<SousService>> GetByPriceRangeAsync(decimal minPrix, decimal maxPrix, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SousService>()
            .Where(s => s.EstDisponible && s.Prix >= minPrix && s.Prix <= maxPrix)
            .Include(s => s.Service)
            .Include(s => s.Disponibilites)
            .ToListAsync(cancellationToken);
    }
}
