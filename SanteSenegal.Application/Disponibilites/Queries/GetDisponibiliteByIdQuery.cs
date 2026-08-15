using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;

namespace SanteSenegal.Application.Disponibilites.Queries;

public record GetDisponibiliteByIdQuery(int Id) : IRequest<DisponibiliteDto>;
