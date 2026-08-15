using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Services.Commands.Handlers;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, bool>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceCommandHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (service is null)
            return false;

        if (request.Nom is not null)
            service.Nom = request.Nom;
        if (request.Description is not null)
            service.Description = request.Description;
        if (request.EstActif.HasValue)
            service.EstActif = request.EstActif.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
