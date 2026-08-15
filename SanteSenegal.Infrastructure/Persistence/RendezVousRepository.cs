using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence;

public class RendezVousRepository : IRendezVousRepository
{
    private readonly SanteDbContext _context;

    public RendezVousRepository(SanteDbContext context)
    {
        _context = context;
    }

    public async Task<RendezVous?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RendezVous
            .Include(r => r.Patient)
            .Include(r => r.Structure)
            .Include(r => r.Service)
            .Include(r => r.SousService)
            .Include(r => r.Disponibilite)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<RendezVous>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.RendezVous
            .Include(r => r.Patient)
            .Include(r => r.Structure)
            .Include(r => r.Service)
            .Include(r => r.SousService)
            .Where(r => r.PatientId == patientId)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.Heure)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RendezVous>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RendezVous
            .Include(r => r.Patient)
            .Include(r => r.Structure)
            .Include(r => r.Service)
            .Include(r => r.SousService)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.Heure)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RendezVous rendezVous, CancellationToken cancellationToken = default)
    {
        await _context.RendezVous.AddAsync(rendezVous, cancellationToken);
    }

    public async Task UpdateAsync(RendezVous rendezVous, CancellationToken cancellationToken = default)
    {
        _context.RendezVous.Update(rendezVous);
        await Task.CompletedTask;
    }
}
