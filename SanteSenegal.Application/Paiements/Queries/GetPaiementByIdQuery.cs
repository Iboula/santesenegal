using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Paiements.Queries;

public record GetPaiementByIdQuery(int Id) : IRequest<Paiement?>;

public class GetPaiementByIdQueryHandler : IRequestHandler<GetPaiementByIdQuery, Paiement?>
{
    private readonly IPaiementRepository _repository;

    public GetPaiementByIdQueryHandler(IPaiementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Paiement?> Handle(GetPaiementByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
