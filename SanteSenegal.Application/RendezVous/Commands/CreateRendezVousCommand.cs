using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Application.DTOs.RendezVous;

namespace SanteSenegal.Application.RendezVous.Commands;

public record CreateRendezVousCommand(CreateRendezVousDto RendezVous) : IRequest<RendezVousDto>;
