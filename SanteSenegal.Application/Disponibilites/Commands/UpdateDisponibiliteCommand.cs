using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;

namespace SanteSenegal.Application.Disponibilites.Commands;

public record UpdateDisponibiliteCommand(int Id, DisponibiliteUpdateDto Disponibilite) : IRequest<bool>;
