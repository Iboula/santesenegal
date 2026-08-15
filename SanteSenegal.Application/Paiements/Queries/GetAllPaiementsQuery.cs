using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Paiements.Queries;

public record GetAllPaiementsQuery : IRequest<List<Paiement>>;

public class GetAllPaiementsQueryHandler : IRequestHandler<GetAllPaiementsQuery, List<Paiement>>
{
    private readonly IPaiementRepository _repository;

    public GetAllPaiementsQueryHandler(IPaiementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Paiement>> Handle(GetAllPaiementsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
