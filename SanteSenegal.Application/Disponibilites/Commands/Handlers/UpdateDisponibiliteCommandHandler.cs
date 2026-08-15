using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Application.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Commands.Handlers;

public class UpdateDisponibiliteCommandHandler : IRequestHandler<UpdateDisponibiliteCommand, bool>
{
    private readonly IDisponibiliteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDisponibiliteCommandHandler(IDisponibiliteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDisponibiliteCommand request, CancellationToken cancellationToken)
    {
        var disponibilite = await _repository.GetByIdAsync(request.Id);
        if (disponibilite == null)
            return false;

        // Update properties from DTO
        if (request.Disponibilite.Date.HasValue)
            disponibilite.Date = request.Disponibilite.Date.Value;
        if (request.Disponibilite.HeureDebut.HasValue)
            disponibilite.HeureDebut = request.Disponibilite.HeureDebut.Value;
        if (request.Disponibilite.HeureFin.HasValue)
            disponibilite.HeureFin = request.Disponibilite.HeureFin.Value;
        if (request.Disponibilite.EstDisponible.HasValue)
            disponibilite.EstDisponible = request.Disponibilite.EstDisponible.Value;
        if (request.Disponibilite.MaxRendezVous.HasValue)
            disponibilite.MaxRendezVous = request.Disponibilite.MaxRendezVous.Value;
        if (request.Disponibilite.Notes != null)
            disponibilite.Notes = request.Disponibilite.Notes;

        await _repository.UpdateAsync(disponibilite);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
