using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Structures.Commands.Handlers;

public class CreateStructureCommandHandler : IRequestHandler<CreateStructureCommand, StructureDto>
{
    private readonly IStructureRepository _structureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStructureCommandHandler(IStructureRepository structureRepository, IUnitOfWork unitOfWork)
    {
        _structureRepository = structureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StructureDto> Handle(CreateStructureCommand request, CancellationToken cancellationToken)
    {
        var structure = new Structure
        {
            Nom = request.Structure.Nom,
            Type = request.Structure.Type,
            Adresse = request.Structure.Adresse,
            Telephone = request.Structure.Telephone,
            Email = request.Structure.Email,
            Description = request.Structure.Description,
            ImageUrl = request.Structure.ImageUrl,
            HorairesOuverture = request.Structure.HorairesOuverture,
            EstActif = true
        };

        await _structureRepository.AddAsync(structure, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StructureDto
        {
            Id = structure.Id,
            Nom = structure.Nom,
            Type = structure.Type,
            Adresse = structure.Adresse,
            Telephone = structure.Telephone,
            Email = structure.Email,
            Description = structure.Description,
            ImageUrl = structure.ImageUrl,
            HorairesOuverture = structure.HorairesOuverture,
            EstActif = structure.EstActif,
            Services = new List<ServiceBasicDto>()
        };
    }
}
