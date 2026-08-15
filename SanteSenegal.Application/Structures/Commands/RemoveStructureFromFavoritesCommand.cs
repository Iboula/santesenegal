using MediatR;

namespace SanteSenegal.Application.Structures.Commands;

public record RemoveStructureFromFavoritesCommand(
    int PatientId,
    int StructureId
) : IRequest<bool>;
