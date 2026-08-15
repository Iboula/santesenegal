using MediatR;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Commands;

public record UpdateServiceCommand(
    int Id,
    string? Nom,
    string? Description,
    bool? EstActif
) : IRequest<bool>;
