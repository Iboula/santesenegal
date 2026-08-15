using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Structures.Queries;

public record GetFavoriteStructuresQuery(int PatientId) : IRequest<IEnumerable<StructureDto>>;
