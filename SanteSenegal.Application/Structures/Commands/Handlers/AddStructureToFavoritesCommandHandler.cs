using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Commands.Handlers;

public class AddStructureToFavoritesCommandHandler : IRequestHandler<AddStructureToFavoritesCommand, bool>
{
    private readonly IStructureRepository _structureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddStructureToFavoritesCommandHandler(IStructureRepository structureRepository, IUnitOfWork unitOfWork)
    {
        _structureRepository = structureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddStructureToFavoritesCommand request, CancellationToken cancellationToken)
    {
        var success = await _structureRepository.AddToFavoritesAsync(request.PatientId, request.StructureId, cancellationToken);
        if (!success) return false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
