using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Domain.Abstractions;

public interface IDisponibiliteRepository : IBaseRepository<Disponibilite>
{
    Task<IEnumerable<Disponibilite>> GetByStructureAsync(int structureId, DateTime dateDebut, DateTime dateFin, CancellationToken cancellationToken = default);
    Task<IEnumerable<Disponibilite>> GetBySousServiceAsync(int sousServiceId, DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Disponibilite>> GetAvailableAsync(int structureId, int sousServiceId, DateTime date, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(int disponibiliteId, CancellationToken cancellationToken = default);
    Task<bool> UpdateDisponibiliteAsync(int disponibiliteId, bool estDisponible, CancellationToken cancellationToken = default);
    Task<int> GetNombreRendezVousAsync(int disponibiliteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSpan>> GetHorairesDisponiblesAsync(int structureId, int sousServiceId, DateTime date, CancellationToken cancellationToken = default);
}
