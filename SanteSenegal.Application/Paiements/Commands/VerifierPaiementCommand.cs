using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Paiements.Commands;

public record VerifierPaiementCommand(int PaiementId) : IRequest<Result<(Paiement Paiement, bool EstPaye)>>;

public class VerifierPaiementCommandHandler : IRequestHandler<VerifierPaiementCommand, Result<(Paiement Paiement, bool EstPaye)>>
{
    private readonly IPaiementService _paiementService;

    public VerifierPaiementCommandHandler(IPaiementService paiementService)
    {
        _paiementService = paiementService;
    }

    public async Task<Result<(Paiement Paiement, bool EstPaye)>> Handle(VerifierPaiementCommand request, CancellationToken cancellationToken)
    {
        return await _paiementService.VerifierPaiementMobileAsync(request.PaiementId);
    }
}
