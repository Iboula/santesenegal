using MediatR;

namespace SanteSenegal.Application.RendezVous.Commands;

public record DeleteRendezVousCommand(int Id) : IRequest<bool>;
