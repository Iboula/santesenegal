using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.Disponibilites.Queries;
using SanteSenegal.Application.DTOs.Disponibilites;
using SanteSenegal.Application.Disponibilites.Commands;

namespace SanteSenegal.Api.Endpoints;

public static class DisponibiliteEndpoints
{
    public static void MapDisponibiliteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/disponibilites");

        // PUBLIC - Liste toutes les disponibilités
        group.MapGet("/", [AllowAnonymous] async (IMediator mediator) =>
        {
            var query = new GetAllDisponibilitesQuery();
            var disponibilites = await mediator.Send(query);
            return Results.Ok(disponibilites);
        })
        .WithName("GetDisponibilites")
        .WithOpenApi();

        // PUBLIC - Obtient une disponibilité par ID
        group.MapGet("/{id}", [AllowAnonymous] async (int id, IMediator mediator) =>
        {
            var query = new GetDisponibiliteByIdQuery(id);
            var disponibilite = await mediator.Send(query);
            return disponibilite is null ? Results.NotFound() : Results.Ok(disponibilite);
        })
        .WithName("GetDisponibiliteById")
        .WithOpenApi();

        // PUBLIC - Liste les disponibilités par structure et sous-service
        group.MapGet("/structure/{structureId}/sous-service/{sousServiceId}", [AllowAnonymous] async (int structureId, int sousServiceId, DateTime date, IMediator mediator) =>
        {
            var query = new GetAvailableDisponibilitesQuery(structureId, sousServiceId, date);
            var disponibilites = await mediator.Send(query);
            return Results.Ok(disponibilites);
        })
        .WithName("GetDisponibilitesByStructureAndSousService")
        .WithOpenApi();

        // PUBLIC - Liste les horaires disponibles
        group.MapGet("/horaires", [AllowAnonymous] async (int structureId, int sousServiceId, DateTime date, IMediator mediator) =>
        {
            var query = new GetHorairesDisponiblesQuery(structureId, sousServiceId, date);
            var horaires = await mediator.Send(query);
            return Results.Ok(horaires);
        })
        .WithName("GetHorairesDisponibles")
        .WithOpenApi();

        // PUBLIC - Liste les disponibilités par structure
        group.MapGet("/structure/{structureId}", [AllowAnonymous] async (int structureId, DateTime dateDebut, DateTime dateFin, IMediator mediator) =>
        {
            if (dateFin < dateDebut)
                return Results.BadRequest("La date de fin doit être supérieure à la date de début");

            var query = new GetDisponibilitesByStructureQuery(structureId, dateDebut, dateFin);
            var disponibilites = await mediator.Send(query);
            return Results.Ok(disponibilites);
        })
        .WithName("GetDisponibilitesByStructure")
        .WithOpenApi();

        // PROTECTED - Crée une nouvelle disponibilité
        group.MapPost("/", [Authorize(Roles = "Medecin,Admin")] async (DisponibiliteCreateDto createDto, IMediator mediator) =>
        {
            if (createDto.HeureFin <= createDto.HeureDebut)
                return Results.BadRequest("L'heure de fin doit être supérieure à l'heure de début");

            var command = new CreateDisponibiliteCommand(createDto);
            var result = await mediator.Send(command);
            return Results.Created($"/api/disponibilites/{result.Id}", result);
        })
        .WithName("CreateDisponibilite")
        .WithOpenApi();

        // PROTECTED - Met à jour une disponibilité
        group.MapPut("/{id}", [Authorize(Roles = "Medecin,Admin")] async (int id, DisponibiliteUpdateDto updateDto, IMediator mediator) =>
        {
            var command = new UpdateDisponibiliteCommand(id, updateDto);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateDisponibilite")
        .WithOpenApi();

        // PUBLIC - Vérifie si une disponibilité est disponible
        group.MapGet("/{id}/disponible", [AllowAnonymous] async (int id, IMediator mediator) =>
        {
            var query = new CheckDisponibiliteQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(new { estDisponible = result });
        })
        .WithName("CheckDisponibilite")
        .WithOpenApi();

        // PROTECTED - Supprime une disponibilité
        group.MapDelete("/{id}", [Authorize(Roles = "Medecin,Admin")] async (int id, IMediator mediator) =>
        {
            var command = new DeleteDisponibiliteCommand(id);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteDisponibilite")
        .WithOpenApi();
    }
}
