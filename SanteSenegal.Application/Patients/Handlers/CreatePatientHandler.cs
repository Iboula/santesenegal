using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Patients.Commands;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Patients.Handlers;

public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, int>
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _uow;

    public CreatePatientHandler(IPatientRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<int> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Nom = command.Nom,
            Prenom = command.Prenom,
            DateNaissance = command.DateNaissance,
            Telephone = command.Telephone
        };

        await _repository.AddAsync(patient, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return patient.Id;
    }
}
