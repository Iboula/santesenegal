using SanteSenegal.Domain.Entities;
using System.Linq.Expressions;

namespace SanteSenegal.Domain.Abstractions;

public interface IBaseRepository<T> where T : BaseEntity, new()
{
    void AddInclusion(Expression<Func<T, Object>> _inclusion);
    void AddCriteria(Func<IQueryable<T>, IQueryable<T>> _critere);
    void AddOrderBy(string propertyName, bool ascendant = true);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}