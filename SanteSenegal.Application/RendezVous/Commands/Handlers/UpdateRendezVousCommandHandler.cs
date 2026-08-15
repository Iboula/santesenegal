using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.RendezVous.Commands.Handlers;

public class UpdateRendezVousCommandHandler : IRequestHandler<UpdateRendezVousCommand, bool>
{
    private readonly IRendezVousRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRendezVousCommandHandler(IRendezVousRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRendezVousCommand request, CancellationToken cancellationToken)
    {
        var rendezVous = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (rendezVous == null)
            return false;

        if (request.RendezVous.Date.HasValue)
            rendezVous.Date = request.RendezVous.Date.Value;
        if (request.RendezVous.Heure.HasValue)
            rendezVous.Heure = request.RendezVous.Heure.Value;
        if (request.RendezVous.Motif != null)
            rendezVous.Motif = request.RendezVous.Motif;
        if (request.RendezVous.Notes != null)
            rendezVous.Notes = request.RendezVous.Notes;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
