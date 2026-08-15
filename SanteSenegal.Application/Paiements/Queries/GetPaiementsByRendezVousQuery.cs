using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Paiements.Queries;

public record GetPaiementsByRendezVousQuery(int RendezVousId) : IRequest<List<Paiement>>;

public class GetPaiementsByRendezVousQueryHandler : IRequestHandler<GetPaiementsByRendezVousQuery, List<Paiement>>
{
    private readonly IPaiementRepository _repository;

    public GetPaiementsByRendezVousQueryHandler(IPaiementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Paiement>> Handle(GetPaiementsByRendezVousQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByRendezVousIdAsync(request.RendezVousId, cancellationToken);
    }
}
