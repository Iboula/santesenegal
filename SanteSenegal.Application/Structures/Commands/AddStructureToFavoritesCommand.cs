using MediatR;

namespace SanteSenegal.Application.Structures.Commands;

public record AddStructureToFavoritesCommand(
    int PatientId,
    int StructureId
) : IRequest<bool>;
