using MediatR;
using SanteSenegal.Application.DTOs.Disponibilites;

namespace SanteSenegal.Application.Disponibilites.Queries;

public record GetDisponibilitesByStructureQuery(
    int StructureId,
    DateTime DateDebut,
    DateTime DateFin) : IRequest<IEnumerable<DisponibiliteDto>>;
