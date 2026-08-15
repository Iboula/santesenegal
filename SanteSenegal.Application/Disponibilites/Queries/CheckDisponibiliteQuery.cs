using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Queries;

public record CheckDisponibiliteQuery(int DisponibiliteId) : IRequest<bool>;

public class CheckDisponibiliteQueryHandler : IRequestHandler<CheckDisponibiliteQuery, bool>
{
    private readonly IDisponibiliteRepository _repository;

    public CheckDisponibiliteQueryHandler(IDisponibiliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CheckDisponibiliteQuery request, CancellationToken cancellationToken)
    {
        return await _repository.IsAvailableAsync(request.DisponibiliteId, cancellationToken);
    }
}
