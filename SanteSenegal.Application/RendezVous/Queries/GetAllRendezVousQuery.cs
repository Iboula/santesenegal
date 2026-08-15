using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.RendezVous.Queries;

public record GetAllRendezVousQuery() : IRequest<IEnumerable<RendezVousDto>>;
