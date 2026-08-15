using MediatR;
using SanteSenegal.Application.DTOs;

namespace SanteSenegal.Application.Services.Queries;

public record GetServiceDisponibiliteQuery(int ServiceId, DateTime Date) : IRequest<bool>;
