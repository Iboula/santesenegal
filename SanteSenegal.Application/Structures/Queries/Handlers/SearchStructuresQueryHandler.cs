using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Queries.Handlers;

public class SearchStructuresQueryHandler : IRequestHandler<SearchStructuresQuery, IEnumerable<StructureDto>>
{
    private readonly IStructureRepository _structureRepository;

    public SearchStructuresQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<IEnumerable<StructureDto>> Handle(SearchStructuresQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Structure> structures;

        if (!string.IsNullOrEmpty(request.Term))
            structures = await _structureRepository.SearchAsync(request.Term, cancellationToken);
        else if (request.Type.HasValue)
            structures = await _structureRepository.GetByTypeAsync(request.Type.Value, cancellationToken);
        else if (request.ServiceType.HasValue)
            structures = await _structureRepository.GetByServiceTypeAsync(request.ServiceType.Value, cancellationToken);
        else
            structures = await _structureRepository.GetAllAsync(cancellationToken);

        return structures.Select(s => new StructureDto
        {
            Id = s.Id,
            Nom = s.Nom,
            Type = s.Type,
            Adresse = s.Adresse,
            Telephone = s.Telephone,
            Email = s.Email,
            Description = s.Description,
            ImageUrl = s.ImageUrl,
            HorairesOuverture = s.HorairesOuverture,
            EstActif = s.EstActif,
            Services = s.Services?.Select(sv => new ServiceBasicDto
            {
                Id = sv.Id,
                Nom = sv.Nom,
                Type = sv.Type
            }).ToList() ?? new List<ServiceBasicDto>()
        });
    }
}
