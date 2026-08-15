using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Persistence;

public class StructureRepository : BaseRepository<Structure>, IStructureRepository
{
    private readonly SanteDbContext _context;

    public StructureRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Structure>> GetByTypeAsync(TypeStructure type, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Structure>()
            .Where(s => s.Type == type && s.EstActif)
            .Include(s => s.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Structure>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Structure>()
            .Where(s => s.EstActif &&
                (s.Nom.Contains(searchTerm) ||
                s.Description!.Contains(searchTerm) ||
                s.Adresse.Contains(searchTerm)))
            .Include(s => s.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Structure>> GetByServiceTypeAsync(TypeService serviceType, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Structure>()
            .Where(s => s.EstActif && s.Services.Any(ser => ser.Type == serviceType))
            .Include(s => s.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasServiceAsync(int structureId, int serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Structure>()
            .Where(s => s.Id == structureId && s.EstActif)
            .AnyAsync(s => s.Services.Any(ser => ser.Id == serviceId), cancellationToken);
    }

    public async Task<IEnumerable<Structure>> GetFavoritesByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<PatientStructureFavorite>()
            .Where(f => f.PatientId == patientId)
            .Include(f => f.Structure)
                .ThenInclude(s => s.Services)
            .Select(f => f.Structure)
            .Where(s => s.EstActif)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AddToFavoritesAsync(int patientId, int structureId, CancellationToken cancellationToken = default)
    {
        var existingFavorite = await _context.Set<PatientStructureFavorite>()
            .FirstOrDefaultAsync(f => f.PatientId == patientId && f.StructureId == structureId, cancellationToken);

        if (existingFavorite != null)
            return true;

        var favorite = new PatientStructureFavorite
        {
            PatientId = patientId,
            StructureId = structureId,
            DateAjout = DateTime.UtcNow
        };

        await _context.Set<PatientStructureFavorite>().AddAsync(favorite, cancellationToken);
        return true;
    }

    public async Task<bool> RemoveFromFavoritesAsync(int patientId, int structureId, CancellationToken cancellationToken = default)
    {
        var favorite = await _context.Set<PatientStructureFavorite>()
            .FirstOrDefaultAsync(f => f.PatientId == patientId && f.StructureId == structureId, cancellationToken);

        if (favorite == null)
            return false;

        _context.Set<PatientStructureFavorite>().Remove(favorite);
        return true;
    }
}
