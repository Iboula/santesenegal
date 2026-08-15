using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Services.Queries.Handlers;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto?>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceByIdQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto?> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (service == null)
            return null;

        return new ServiceDto
        {
            Id = service.Id,
            Nom = service.Nom,
            Type = service.Type,
            Description = service.Description,
            EstActif = service.EstActif,
            SousServices = service.SousServices?.Select(ss => new SousServiceBasicDto
            {
                Id = ss.Id,
                Nom = ss.Nom,
                Prix = ss.Prix,
                Specialite = ss.Specialite
            }).ToList() ?? new List<SousServiceBasicDto>()
        };
    }
}
