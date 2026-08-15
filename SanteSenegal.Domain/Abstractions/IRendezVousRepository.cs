using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Domain.Abstractions;

public interface IRendezVousRepository
{
    Task<RendezVous?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RendezVous>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RendezVous>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RendezVous rendezVous, CancellationToken cancellationToken = default);
    Task UpdateAsync(RendezVous rendezVous, CancellationToken cancellationToken = default);
}
