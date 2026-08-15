using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Services.Queries.Handlers;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, IEnumerable<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllAsync(cancellationToken);
        return services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Nom = s.Nom,
            Type = s.Type,
            Description = s.Description,
            EstActif = s.EstActif,
            SousServices = s.SousServices?.Select(ss => new SousServiceBasicDto
            {
                Id = ss.Id,
                Nom = ss.Nom,
                Prix = ss.Prix,
                Specialite = ss.Specialite
            }).ToList() ?? new List<SousServiceBasicDto>()
        });
    }
}
