using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.RendezVous.Queries;

public record GetRendezVousByPatientQuery(int PatientId) : IRequest<IEnumerable<RendezVousDto>>;
