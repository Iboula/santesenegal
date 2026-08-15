using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Paiements.Queries;

public record GetPaiementsByPatientQuery(int PatientId) : IRequest<List<Paiement>>;

public class GetPaiementsByPatientQueryHandler : IRequestHandler<GetPaiementsByPatientQuery, List<Paiement>>
{
    private readonly IPaiementRepository _repository;

    public GetPaiementsByPatientQueryHandler(IPaiementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Paiement>> Handle(GetPaiementsByPatientQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByPatientIdAsync(request.PatientId, cancellationToken);
    }
}
