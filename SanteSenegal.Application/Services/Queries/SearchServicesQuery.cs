using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Queries;

public record SearchServicesQuery(string SearchTerm) : IRequest<IEnumerable<ServiceDto>>;
