using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Structures.Queries.Handlers;

public class GetFavoriteStructuresQueryHandler : IRequestHandler<GetFavoriteStructuresQuery, IEnumerable<StructureDto>>
{
    private readonly IStructureRepository _structureRepository;

    public GetFavoriteStructuresQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<IEnumerable<StructureDto>> Handle(GetFavoriteStructuresQuery request, CancellationToken cancellationToken)
    {
        var structures = await _structureRepository.GetFavoritesByPatientIdAsync(request.PatientId, cancellationToken);
        
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
