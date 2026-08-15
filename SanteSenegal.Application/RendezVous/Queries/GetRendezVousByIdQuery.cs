using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.RendezVous.Queries;

public record GetRendezVousByIdQuery(int Id) : IRequest<RendezVousDto?>;
