using MediatR;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.RendezVous.Commands;

public record UpdateRendezVousStatutCommand(
    int Id,
    StatutRendezVous Statut
) : IRequest<bool>;
