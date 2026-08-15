using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Queries;

public record GetAllServicesQuery() : IRequest<IEnumerable<ServiceDto>>;
