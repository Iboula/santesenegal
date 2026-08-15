using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Structures.Commands;

public record CreateStructureCommand(StructureCreateDto Structure) : IRequest<StructureDto>;
