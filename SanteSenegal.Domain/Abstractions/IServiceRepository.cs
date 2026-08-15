using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Abstractions;

public interface IServiceRepository : IBaseRepository<Service>
{
    Task<IEnumerable<Service>> GetByStructureIdAsync(int structureId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetByTypeAsync(TypeService type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> IsServiceAvailableAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetActiveServicesAsync(int structureId, CancellationToken cancellationToken = default);
}
