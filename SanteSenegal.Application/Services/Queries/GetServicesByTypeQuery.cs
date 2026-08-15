using MediatR;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Queries;

public record GetServicesByTypeQuery(TypeService Type) : IRequest<IEnumerable<ServiceDto>>;
