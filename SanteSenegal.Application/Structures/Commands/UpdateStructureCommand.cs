using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Structures.Commands;

public record UpdateStructureCommand(int Id, StructureUpdateDto Structure) : IRequest<bool>;
