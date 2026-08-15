using SanteSenegal.Application.Abstractions;

namespace SanteSenegal.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SanteDbContext _context;

    public UnitOfWork(SanteDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
