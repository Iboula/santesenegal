using MediatR;
using SanteSenegal.Domain.Abstractions;

namespace SanteSenegal.Application.Services.Queries.Handlers;

public class GetServiceDisponibiliteQueryHandler : IRequestHandler<GetServiceDisponibiliteQuery, bool>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceDisponibiliteQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<bool> Handle(GetServiceDisponibiliteQuery request, CancellationToken cancellationToken)
    {
        return await _serviceRepository.IsServiceAvailableAsync(request.ServiceId, request.Date, cancellationToken);
    }
}
