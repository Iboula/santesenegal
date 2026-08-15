using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Structures.Queries;

public record SearchStructuresQuery(string? Term, TypeStructure? Type, TypeService? ServiceType) : IRequest<IEnumerable<StructureDto>>;
