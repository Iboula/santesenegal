using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Domain.Abstractions;

public interface IPaiementRepository : IBaseRepository<Paiement>
{
    Task<List<Paiement>> GetByRendezVousIdAsync(int rendezVousId, CancellationToken cancellationToken = default);
    Task<List<Paiement>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
}
