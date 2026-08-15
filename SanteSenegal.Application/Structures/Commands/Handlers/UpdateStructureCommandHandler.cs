using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Commands.Handlers;

public class UpdateStructureCommandHandler : IRequestHandler<UpdateStructureCommand, bool>
{
    private readonly IStructureRepository _structureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStructureCommandHandler(IStructureRepository structureRepository, IUnitOfWork unitOfWork)
    {
        _structureRepository = structureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStructureCommand request, CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetByIdAsync(request.Id, cancellationToken);
        if (structure == null)
            return false;

        if (request.Structure.Nom != null)
            structure.Nom = request.Structure.Nom;
        if (request.Structure.Adresse != null)
            structure.Adresse = request.Structure.Adresse;
        if (request.Structure.Telephone != null)
            structure.Telephone = request.Structure.Telephone;
        if (request.Structure.Email != null)
            structure.Email = request.Structure.Email;
        if (request.Structure.Description != null)
            structure.Description = request.Structure.Description;
        if (request.Structure.ImageUrl != null)
            structure.ImageUrl = request.Structure.ImageUrl;
        if (request.Structure.HorairesOuverture != null)
            structure.HorairesOuverture = request.Structure.HorairesOuverture;
        if (request.Structure.EstActif.HasValue)
            structure.EstActif = request.Structure.EstActif.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
