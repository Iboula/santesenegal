using MediatR;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Patients.Queries;

public record GetPatientByIdQuery(int Id) : IRequest<Patient?>;
