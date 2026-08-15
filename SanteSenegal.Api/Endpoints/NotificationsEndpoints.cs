using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs.Notifications;
using SanteSenegal.Application.Notifications.Commands;
using SanteSenegal.Application.Notifications.Queries;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications");

        // ─── ENVOI SMS DIRECT ───
        group.MapPost("/sms", [Authorize(Roles = "Admin,AgentSante")] async (
            EnvoyerSMSRequestDto dto,
            IMediator mediator) =>
        {
            var command = new EnvoyerSMSCommand(dto);
            var result = await mediator.Send(command);
            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });
            return Results.Ok(result.Value);
        });

        // ─── RAPPEL RENDEZ-VOUS ───
        group.MapPost("/rappel-rdv/{rendezVousId}", [Authorize(Roles = "Admin,AgentSante")] async (
            int rendezVousId,
            string? langue,
            IMediator mediator) =>
        {
            var command = new EnvoyerRappelRendezVousCommand(rendezVousId, langue ?? "fr");
            var result = await mediator.Send(command);
            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });
            return Results.Ok(result.Value);
        });

        // ─── CONFIRMATION PAIEMENT ───
        group.MapPost("/confirmation-paiement/{paiementId}", [Authorize(Roles = "Admin,AgentSante")] async (
            int paiementId,
            string? langue,
            IMediator mediator) =>
        {
            var command = new EnvoyerConfirmationPaiementCommand(paiementId, langue ?? "fr");
            var result = await mediator.Send(command);
            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });
            return Results.Ok(result.Value);
        });

        // ─── ALERTE EPIDEMIOLOGIQUE (BROADCAST) ───
        group.MapPost("/alerte-epidemio", [Authorize(Roles = "Admin")] async (
            AlerteEpidemiologiqueRequestDto dto,
            IMediator mediator) =>
        {
            var command = new EnvoyerAlerteEpidemiologiqueCommand(dto);
            var result = await mediator.Send(command);
            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });
            return Results.Ok(new { envoyeAvecSucces = result.Value, total = dto.Telephones.Count });
        });

        // ─── LISTE TOUTES LES NOTIFICATIONS ───
        group.MapGet("/", [Authorize(Roles = "Admin")] async (IMediator mediator) =>
        {
            var query = new GetNotificationsQuery();
            var notifications = await mediator.Send(query);
            return Results.Ok(notifications);
        });

        // ─── LISTE PAR STATUT ───
        group.MapGet("/statut/{statut}", [Authorize(Roles = "Admin,AgentSante")] async (
            string statut,
            IMediator mediator) =>
        {
            if (!Enum.TryParse<StatutNotification>(statut, true, out var statutEnum))
                return Results.BadRequest(new { error = "Statut invalide" });

            var query = new GetNotificationsByStatutQuery(statutEnum);
            var notifications = await mediator.Send(query);
            return Results.Ok(notifications);
        });

        // ─── RAPPELS AUTOMATIQUES (RDV dans 24h) ───
        group.MapPost("/rappels-auto", [Authorize(Roles = "Admin,AgentSante")] async (
            IMediator mediator) =>
        {
            // Cet endpoint serait typiquement appelé par un cron job
            // Pour l'instant on retourne une confirmation
            return Results.Ok(new { message = "Les rappels automatiques sont gérés par un background job. Utilisez /rappel-rdv/{id} pour envoyer manuellement." });
        });
    }
}
