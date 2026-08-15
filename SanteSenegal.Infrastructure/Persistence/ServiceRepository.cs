using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Persistence;

public class ServiceRepository : BaseRepository<Service>, IServiceRepository
{
    private readonly SanteDbContext _context;

    public ServiceRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetByStructureIdAsync(int structureId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .Where(s => s.StructureId == structureId && s.EstActif)
            .Include(s => s.SousServices)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetByTypeAsync(TypeService type, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .Where(s => s.Type == type && s.EstActif)
            .Include(s => s.SousServices)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Service>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .Where(s => s.EstActif &&
                (s.Nom.Contains(searchTerm) ||
                s.Description.Contains(searchTerm)))
            .Include(s => s.SousServices)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsServiceAvailableAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .Where(s => s.Id == serviceId && s.EstActif)
            .AnyAsync(s => s.SousServices.Any(ss => 
                ss.EstDisponible && 
                ss.Disponibilites.Any(d => 
                    d.Date.Date == date.Date && 
                    d.EstDisponible)), 
                cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetActiveServicesAsync(int structureId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .Where(s => s.StructureId == structureId && 
                s.EstActif && 
                s.SousServices.Any(ss => ss.EstDisponible))
            .Include(s => s.SousServices)
            .ToListAsync(cancellationToken);
    }
}
