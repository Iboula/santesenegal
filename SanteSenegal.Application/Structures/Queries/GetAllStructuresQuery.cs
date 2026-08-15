using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Structures.Queries;

public record GetAllStructuresQuery() : IRequest<IEnumerable<StructureDto>>;
