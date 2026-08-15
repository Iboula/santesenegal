using SanteSenegal.Application.Abstractions;
using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Disponibilites.Commands.Handlers;

public class CreateDisponibiliteCommandHandler : IRequestHandler<CreateDisponibiliteCommand, DisponibiliteDto>
{
    private readonly IDisponibiliteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDisponibiliteCommandHandler(IDisponibiliteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DisponibiliteDto> Handle(CreateDisponibiliteCommand request, CancellationToken cancellationToken)
    {
        var disponibilite = new Disponibilite
        {
            Date = request.Disponibilite.Date,
            HeureDebut = request.Disponibilite.HeureDebut,
            HeureFin = request.Disponibilite.HeureFin,
            EstDisponible = true,
            MaxRendezVous = request.Disponibilite.MaxRendezVous,
            Notes = request.Disponibilite.Notes,
            StructureId = request.Disponibilite.StructureId,
            SousServiceId = request.Disponibilite.SousServiceId
        };

        await _repository.AddAsync(disponibilite);
        await _unitOfWork.SaveChangesAsync();

        return new DisponibiliteDto
        {
            Id = disponibilite.Id,
            Date = disponibilite.Date,
            HeureDebut = disponibilite.HeureDebut,
            HeureFin = disponibilite.HeureFin,
            EstDisponible = disponibilite.EstDisponible,
            MaxRendezVous = disponibilite.MaxRendezVous,
            NombreRendezVousPris = 0,
            Notes = disponibilite.Notes
        };
    }
}
