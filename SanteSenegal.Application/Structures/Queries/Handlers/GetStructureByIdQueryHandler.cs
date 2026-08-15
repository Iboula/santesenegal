using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Queries.Handlers;

public class GetStructureByIdQueryHandler : IRequestHandler<GetStructureByIdQuery, StructureDto?>
{
    private readonly IStructureRepository _structureRepository;

    public GetStructureByIdQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<StructureDto?> Handle(GetStructureByIdQuery request, CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetByIdAsync(request.Id, cancellationToken);
        if (structure == null) return null;

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
            Services = structure.Services?.Select(s => new ServiceBasicDto
            {
                Id = s.Id,
                Nom = s.Nom,
                Type = s.Type
            }).ToList() ?? new List<ServiceBasicDto>()
        };
    }
}
