using MediatR;
using SanteSenegal.Application.DTOs.RendezVous;

namespace SanteSenegal.Application.RendezVous.Commands;

public record UpdateRendezVousCommand(int Id, UpdateRendezVousDto RendezVous) : IRequest<bool>;
