using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Structures.Queries;

public record GetStructureByIdQuery(int Id) : IRequest<StructureDto?>;
