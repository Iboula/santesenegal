using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using SanteSenegal.Application.RendezVous.Commands;
using SanteSenegal.Application.RendezVous.Queries;
using SanteSenegal.Application.DTOs.RendezVous;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Api.Endpoints;

public static class RendezVousEndpoints
{
    public static void MapRendezVousEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rendez-vous");

        group.MapGet("/patient/{patientId}", [Authorize] async (int patientId, IMediator mediator) =>
        {
            var query = new GetRendezVousByPatientQuery(patientId);
            var rendezVous = await mediator.Send(query);
            return Results.Ok(rendezVous);
        });

        group.MapGet("/{id}", [Authorize] async (int id, IMediator mediator) =>
        {
            var query = new GetRendezVousByIdQuery(id);
            var rendezVous = await mediator.Send(query);
            return rendezVous is null ? Results.NotFound() : Results.Ok(rendezVous);
        });

        group.MapPost("/", [Authorize(Roles = "Patient,Admin")] async (CreateRendezVousDto createDto, IMediator mediator) =>
        {
            var command = new CreateRendezVousCommand(createDto);
            var result = await mediator.Send(command);
            return Results.Created($"/api/rendez-vous/{result.Id}", result);
        });

        group.MapPut("/{id}", [Authorize(Roles = "Medecin,Admin")] async (int id, UpdateRendezVousDto updateDto, IMediator mediator) =>
        {
            var command = new UpdateRendezVousCommand(id, updateDto);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id}", [Authorize(Roles = "Admin")] async (int id, IMediator mediator) =>
        {
            var command = new DeleteRendezVousCommand(id);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        });

        group.MapPut("/{id}/status", [Authorize(Roles = "Medecin,Admin")] async (int id, [FromBody] StatutRendezVous newStatus, IMediator mediator) =>
        {
            var command = new UpdateRendezVousStatutCommand(id, newStatus);
            var result = await mediator.Send(command);
            return result ? Results.NoContent() : Results.NotFound();
        });
    }
}
