using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Patients.Queries;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Patients.Handlers;

public class GetPatientByIdHandler : IRequestHandler<GetPatientByIdQuery, Patient?>
{
    private readonly IPatientRepository _repository;

    public GetPatientByIdHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Patient?> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(query.Id, cancellationToken);
}
