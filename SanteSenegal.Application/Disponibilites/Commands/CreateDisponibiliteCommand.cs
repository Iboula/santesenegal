using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;

namespace SanteSenegal.Application.Disponibilites.Commands;

public record CreateDisponibiliteCommand(DisponibiliteCreateDto Disponibilite) : IRequest<DisponibiliteDto>;
