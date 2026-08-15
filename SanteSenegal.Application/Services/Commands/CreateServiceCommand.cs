using MediatR;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Services.Commands;

public record CreateServiceCommand(
    string Nom,
    TypeService Type,
    string? Description
) : IRequest<ServiceDto>;
