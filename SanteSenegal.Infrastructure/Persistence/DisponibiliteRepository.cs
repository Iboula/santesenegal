using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence;

public class DisponibiliteRepository : BaseRepository<Disponibilite>, IDisponibiliteRepository
{
    private readonly SanteDbContext _context;

    public DisponibiliteRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Disponibilite>> GetByStructureAsync(int structureId, DateTime dateDebut, DateTime dateFin, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Disponibilite>()
            .Where(d => d.StructureId == structureId &&
                d.Date.Date >= dateDebut.Date &&
                d.Date.Date <= dateFin.Date)
            .Include(d => d.SousService)
            .OrderBy(d => d.Date)
            .ThenBy(d => d.HeureDebut)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Disponibilite>> GetBySousServiceAsync(int sousServiceId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Disponibilite>()
            .Where(d => d.SousServiceId == sousServiceId &&
                d.Date.Date == date.Date)
            .OrderBy(d => d.HeureDebut)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Disponibilite>> GetAvailableAsync(int structureId, int sousServiceId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Disponibilite>()
            .Where(d => d.StructureId == structureId &&
                d.SousServiceId == sousServiceId &&
                d.Date.Date == date.Date &&
                d.EstDisponible &&
                (!d.MaxRendezVous.HasValue || d.RendezVous.Count < d.MaxRendezVous.Value))
            .OrderBy(d => d.HeureDebut)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsAvailableAsync(int disponibiliteId, CancellationToken cancellationToken = default)
    {
        var disponibilite = await _context.Set<Disponibilite>()
            .Include(d => d.RendezVous)
            .FirstOrDefaultAsync(d => d.Id == disponibiliteId, cancellationToken);

        if (disponibilite == null)
            return false;

        return disponibilite.EstDisponible &&
            (!disponibilite.MaxRendezVous.HasValue || disponibilite.RendezVous.Count < disponibilite.MaxRendezVous.Value);
    }

    public async Task<bool> UpdateDisponibiliteAsync(int disponibiliteId, bool estDisponible, CancellationToken cancellationToken = default)
    {
        var disponibilite = await _context.Set<Disponibilite>()
            .FirstOrDefaultAsync(d => d.Id == disponibiliteId, cancellationToken);

        if (disponibilite == null)
            return false;

        disponibilite.EstDisponible = estDisponible;
        return true;
    }

    public async Task<int> GetNombreRendezVousAsync(int disponibiliteId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RendezVous>()
            .CountAsync(r => r.DisponibiliteId == disponibiliteId, cancellationToken);
    }

    public async Task<IEnumerable<TimeSpan>> GetHorairesDisponiblesAsync(int structureId, int sousServiceId, DateTime date, CancellationToken cancellationToken = default)
    {
        var disponibilites = await GetAvailableAsync(structureId, sousServiceId, date, cancellationToken);
        return disponibilites.Select(d => d.HeureDebut).OrderBy(h => h);
    }
}
