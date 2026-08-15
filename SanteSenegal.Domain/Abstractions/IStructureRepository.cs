using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Abstractions;

public interface IStructureRepository : IBaseRepository<Structure>
{
    Task<IEnumerable<Structure>> GetByTypeAsync(TypeStructure type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Structure>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Structure>> GetByServiceTypeAsync(TypeService serviceType, CancellationToken cancellationToken = default);
    Task<bool> HasServiceAsync(int structureId, int serviceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Structure>> GetFavoritesByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> AddToFavoritesAsync(int patientId, int structureId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromFavoritesAsync(int patientId, int structureId, CancellationToken cancellationToken = default);
}
