using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.RendezVous.Commands.Handlers;

public class DeleteRendezVousCommandHandler : IRequestHandler<DeleteRendezVousCommand, bool>
{
    private readonly IRendezVousRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRendezVousCommandHandler(IRendezVousRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRendezVousCommand request, CancellationToken cancellationToken)
    {
        var rendezVous = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (rendezVous == null)
            return false;

        // Soft delete via statut annulé
        rendezVous.Statut = Domain.Enums.StatutRendezVous.Annule;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
