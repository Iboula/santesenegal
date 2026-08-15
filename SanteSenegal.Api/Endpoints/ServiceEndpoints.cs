using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Application.Services.Commands;
using SanteSenegal.Application.Services.Queries;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Api.Endpoints;

public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/services");

        // PUBLIC - Liste tous les services
        group.MapGet("/", [AllowAnonymous] async (IMediator mediator) =>
        {
            var services = await mediator.Send(new GetAllServicesQuery());
            return Results.Ok(services);
        })
        .WithName("GetServices")
        .WithOpenApi();

        // PUBLIC - Obtient un service par ID
        group.MapGet("/{id}", [AllowAnonymous] async (int id, IMediator mediator) =>
        {
            var service = await mediator.Send(new GetServiceByIdQuery(id));
            return service is null ? Results.NotFound() : Results.Ok(service);
        })
        .WithName("GetServiceById")
        .WithOpenApi();

        // PUBLIC - Liste les services par type
        group.MapGet("/type/{type}", [AllowAnonymous] async (TypeService type, IMediator mediator) =>
        {
            var services = await mediator.Send(new GetServicesByTypeQuery(type));
            return Results.Ok(services);
        })
        .WithName("GetServicesByType")
        .WithOpenApi();

        // PUBLIC - Recherche des services
        group.MapGet("/search", [AllowAnonymous] async (string term, IMediator mediator) =>
        {
            var services = await mediator.Send(new SearchServicesQuery(term));
            return Results.Ok(services);
        })
        .WithName("SearchServices")
        .WithOpenApi();

        // PROTECTED - Crée un nouveau service
        group.MapPost("/", [Authorize(Roles = "Admin")] async (ServiceCreateDto createDto, IMediator mediator) =>
        {
            var service = await mediator.Send(new CreateServiceCommand(
                createDto.Nom,
                createDto.Type,
                createDto.Description
            ));
            
            return Results.Created($"/api/services/{service.Id}", service);
        })
        .WithName("CreateService")
        .WithOpenApi();

        // PROTECTED - Met à jour un service
        group.MapPut("/{id}", [Authorize(Roles = "Admin")] async (int id, ServiceUpdateDto updateDto, IMediator mediator) =>
        {
            var success = await mediator.Send(new UpdateServiceCommand(
                id,
                updateDto.Nom,
                updateDto.Description,
                updateDto.EstActif
            ));

            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateService")
        .WithOpenApi();

        // PUBLIC - Vérifie la disponibilité d'un service
        group.MapGet("/{id}/disponibilite", [AllowAnonymous] async (int id, DateTime date, IMediator mediator) =>
        {
            var isAvailable = await mediator.Send(new GetServiceDisponibiliteQuery(id, date));
            return Results.Ok(new { EstDisponible = isAvailable });
        })
        .WithName("CheckServiceAvailability")
        .WithOpenApi();
    }
}
