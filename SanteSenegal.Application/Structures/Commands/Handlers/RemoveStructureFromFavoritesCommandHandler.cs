using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Commands.Handlers;

public class RemoveStructureFromFavoritesCommandHandler : IRequestHandler<RemoveStructureFromFavoritesCommand, bool>
{
    private readonly IStructureRepository _structureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveStructureFromFavoritesCommandHandler(IStructureRepository structureRepository, IUnitOfWork unitOfWork)
    {
        _structureRepository = structureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveStructureFromFavoritesCommand request, CancellationToken cancellationToken)
    {
        var success = await _structureRepository.RemoveFromFavoritesAsync(request.PatientId, request.StructureId, cancellationToken);
        if (!success) return false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
