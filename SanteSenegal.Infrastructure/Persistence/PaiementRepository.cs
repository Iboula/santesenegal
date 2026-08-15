using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Infrastructure.Persistence;

public class PaiementRepository : BaseRepository<Paiement>, IPaiementRepository
{
    private readonly SanteDbContext _context;

    public PaiementRepository(SanteDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Paiement>> GetByRendezVousIdAsync(int rendezVousId, CancellationToken cancellationToken = default)
    {
        return await _context.Paiements
            .AsNoTracking()
            .Where(p => p.RendezVousId == rendezVousId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Paiement>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Paiements
            .AsNoTracking()
            .Include(p => p.RendezVous)
            .Where(p => p.RendezVous.PatientId == patientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
