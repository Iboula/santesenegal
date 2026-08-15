using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Queries;

public record GetServiceByIdQuery(int Id) : IRequest<ServiceDto?>;
