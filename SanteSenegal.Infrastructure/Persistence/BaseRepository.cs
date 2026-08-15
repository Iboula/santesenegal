using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using System.Linq.Expressions;

namespace SanteSenegal.Infrastructure.Persistence;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
{
    private readonly SanteDbContext _context;
    private Expression<Func<T, object>>? _inclusion = null;
    private Func<IQueryable<T>, IQueryable<T>>? _critere = null;
    private Func<IQueryable<T>, IQueryable<T>>? _orderBy = null;

    public BaseRepository(SanteDbContext context) => _context = context;

    private IQueryable<T> ApplyCriteria()
    {
        var dbSet = _context
                           .Set<T>()
                           .AsQueryable();

        if (_inclusion is not null)
            dbSet = dbSet.Include(_inclusion);

        if (_critere is not null)
            dbSet = _critere(dbSet);

        if (_orderBy is not null)
            dbSet = _orderBy(dbSet);

        return dbSet;
    }

    public void AddInclusion(Expression<Func<T, object>> inclusion)
    {
        _inclusion = inclusion;
    }

    public void AddCriteria(Func<IQueryable<T>, IQueryable<T>> critere)
    {
        _critere = critere;
    }

    public void AddOrderBy(string nomPropriete, bool ascendant = true)
    {
        _orderBy = query =>
        {
            // Récupérer la propriété à partir du nom
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, nomPropriete);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            // Appliquer l'ordre ascendant ou descendant
            return ascendant ? query.OrderBy(lambda) : query.OrderByDescending(lambda);
        };
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dbSet = ApplyCriteria();
        return await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = ApplyCriteria();
        return await dbSet.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
}
