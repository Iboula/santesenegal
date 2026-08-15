using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Domain.Abstractions;

public interface ISousServiceRepository : IBaseRepository<SousService>
{
    Task<IEnumerable<SousService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SousService>> GetBySpecialiteAsync(string specialite, CancellationToken cancellationToken = default);
    Task<IEnumerable<SousService>> GetAvailableAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<SousService>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<decimal> GetPrixAsync(int sousServiceId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrixAsync(int sousServiceId, decimal nouveauPrix, CancellationToken cancellationToken = default);
    Task<IEnumerable<SousService>> GetByPriceRangeAsync(decimal minPrix, decimal maxPrix, CancellationToken cancellationToken = default);
}
