using MediatR;

namespace SanteSenegal.Application.Disponibilites.Commands;

public record DeleteDisponibiliteCommand(int Id) : IRequest<bool>;
