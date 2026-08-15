using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Application.Structures.Commands;
using SanteSenegal.Application.Structures.Queries;

namespace SanteSenegal.Api.Endpoints;

public static class StructureEndpoints
{
    public static void MapStructureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/structures");

        // Liste toutes les structures - PUBLIC
        group.MapGet("/", [AllowAnonymous] async (IMediator mediator) =>
        {
            var structures = await mediator.Send(new GetAllStructuresQuery());
            return Results.Ok(structures);
        })
        .WithName("GetStructures")
        .WithOpenApi();

        // Obtient une structure par ID - PUBLIC
        group.MapGet("/{id}", [AllowAnonymous] async (int id, IMediator mediator) =>
        {
            var structure = await mediator.Send(new GetStructureByIdQuery(id));
            return structure is null ? Results.NotFound() : Results.Ok(structure);
        })
        .WithName("GetStructureById")
        .WithOpenApi();

        // Recherche des structures - PUBLIC
        group.MapGet("/search", [AllowAnonymous] async (string? term, TypeStructure? type, TypeService? serviceType, IMediator mediator) =>
        {
            var query = new SearchStructuresQuery(term, type, serviceType);
            var structures = await mediator.Send(query);
            return Results.Ok(structures);
        })
        .WithName("SearchStructures")
        .WithOpenApi();

        // Crée une nouvelle structure - ADMIN uniquement
        group.MapPost("/", [Authorize(Roles = "Admin")] async (StructureCreateDto createDto, IMediator mediator) =>
        {
            var command = new CreateStructureCommand(createDto);
            var result = await mediator.Send(command);
            return Results.Created($"/api/structures/{result.Id}", result);
        })
        .WithName("CreateStructure")
        .WithOpenApi();

        // Met à jour une structure - ADMIN
        group.MapPut("/{id}", [Authorize(Roles = "Admin")] async (int id, StructureUpdateDto updateDto, IMediator mediator) =>
        {
            var command = new UpdateStructureCommand(id, updateDto);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateStructure")
        .WithOpenApi();

        // Gestion des favoris - AUTHENTIFIÉ
        group.MapPost("/{id}/favoris", [Authorize(Roles = "Patient")] async (int id, int patientId, IMediator mediator) =>
        {
            var command = new AddStructureToFavoritesCommand(id, patientId);
            var success = await mediator.Send(command);
            return success ? Results.Ok() : Results.BadRequest();
        })
        .WithName("AddStructureToFavorites")
        .WithOpenApi();

        group.MapDelete("/{id}/favoris", [Authorize(Roles = "Patient")] async (int id, int patientId, IMediator mediator) =>
        {
            var command = new RemoveStructureFromFavoritesCommand(id, patientId);
            var success = await mediator.Send(command);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("RemoveStructureFromFavorites")
        .WithOpenApi();

        group.MapGet("/favoris/{patientId}", [Authorize(Roles = "Patient")] async (int patientId, IMediator mediator) =>
        {
            var query = new GetFavoriteStructuresQuery(patientId);
            var structures = await mediator.Send(query);
            return Results.Ok(structures);
        })
        .WithName("GetFavoriteStructures")
        .WithOpenApi();
    }
}
