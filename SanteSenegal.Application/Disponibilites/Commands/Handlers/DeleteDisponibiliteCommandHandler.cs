using MediatR;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Application.Abstractions;

namespace SanteSenegal.Application.Disponibilites.Commands.Handlers;

public class DeleteDisponibiliteCommandHandler : IRequestHandler<DeleteDisponibiliteCommand, bool>
{
    private readonly IDisponibiliteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDisponibiliteCommandHandler(IDisponibiliteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteDisponibiliteCommand request, CancellationToken cancellationToken)
    {
        var disponibilite = await _repository.GetByIdAsync(request.Id);
        if (disponibilite == null)
            return false;

        var nombreRendezVous = await _repository.GetNombreRendezVousAsync(request.Id);
        if (nombreRendezVous > 0)
            return false;

        await _repository.DeleteAsync(disponibilite);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
