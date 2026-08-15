using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.Patients.Commands;
using SanteSenegal.Application.Patients.Queries;

namespace SanteSenegal.Api.Endpoints;

public static class PatientsEndpoints
{
    public static void MapPatientsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients", [Authorize(Roles = "Patient,Admin")] async (CreatePatientCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(cmd, ct);
            return Results.Created($"/api/patients/{id}", id);
        });

        app.MapGet("/api/patients/{id:int}", [Authorize] async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var patient = await mediator.Send(new GetPatientByIdQuery(id), ct);
            return patient is not null ? Results.Ok(patient) : Results.NotFound();
        });
    }
}
