using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Services.Commands.Handlers;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new Service
        {
            Nom = request.Nom,
            Type = request.Type,
            Description = request.Description,
            EstActif = true
        };

        await _serviceRepository.AddAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ServiceDto
        {
            Id = service.Id,
            Nom = service.Nom,
            Type = service.Type,
            Description = service.Description,
            EstActif = service.EstActif,
            SousServices = new List<SousServiceBasicDto>()
        };
    }
}
