using MediatR;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs.Paiements;
using SanteSenegal.Application.Paiements.Commands;
using SanteSenegal.Application.Paiements.Queries;

namespace SanteSenegal.Api.Endpoints;

public static class PaiementsEndpoints
{
    public static void MapPaiementsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/paiements");

        // ─── CONFIG ───
        group.MapGet("/modes", () =>
        {
            var modes = new[]
            {
                new PaiementMobileConfigDto(
                    Operateur: "wave",
                    Nom: "Wave",
                    Disponible: true,
                    LogoUrl: "/assets/logos/wave.svg",
                    FraisFixe: 0,
                    FraisPourcentage: 1.0m
                ),
                new PaiementMobileConfigDto(
                    Operateur: "orangemoney",
                    Nom: "Orange Money",
                    Disponible: true,
                    LogoUrl: "/assets/logos/orangemoney.svg",
                    FraisFixe: null,
                    FraisPourcentage: 1.5m
                ),
                new PaiementMobileConfigDto(
                    Operateur: "freemoney",
                    Nom: "Free Money",
                    Disponible: true,
                    LogoUrl: "/assets/logos/freemoney.svg",
                    FraisFixe: null,
                    FraisPourcentage: 1.5m
                )
            };
            return Results.Ok(modes);
        });

        // ─── INITIER PAIEMENT MOBILE ───
        group.MapPost("/initier", [Authorize(Roles = "Patient,Admin")] async (
            PaiementInitierRequestDto dto,
            IMediator mediator) =>
        {
            var createDto = new PaiementCreateDto(
                dto.RendezVousId,
                dto.Montant,
                dto.ModePaiement,
                dto.NumeroTelephone,
                dto.Description
            );
            var command = new ProcessPaiementCommand(createDto);
            var result = await mediator.Send(command);

            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });

            var p = result.Value!;
            var response = new PaiementInitierResponseDto(
                PaiementId: p.Id,
                Statut: p.Statut.ToString(),
                ReferenceExterne: p.ReferenceExterne,
                UrlPaiement: p.UrlPaiement,
                CodeConfirmation: p.CodeConfirmation,
                DateExpiration: p.DateExpiration,
                Message: $"Paiement {p.ModePaiement} initié. Suivez les instructions sur votre téléphone."
            );

            return Results.Ok(response);
        });

        // ─── VÉRIFIER PAIEMENT ───
        group.MapPost("/verifier", [Authorize(Roles = "Patient,Admin")] async (
            PaiementVerifierRequestDto dto,
            IMediator mediator) =>
        {
            var command = new VerifierPaiementCommand(dto.PaiementId);
            var result = await mediator.Send(command);

            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });

            var (paiement, estPaye) = result.Value!;
            var response = new PaiementVerifierResponseDto(
                PaiementId: paiement.Id,
                EstPaye: estPaye,
                Statut: paiement.Statut.ToString(),
                MontantPaye: estPaye ? paiement.Montant : null,
                DateVerification: DateTime.UtcNow,
                Message: estPaye
                    ? "Paiement confirmé avec succès !"
                    : "Paiement en attente de confirmation."
            );

            return Results.Ok(response);
        });

        // ─── CRUD / LISTE EXISTANTE ───
        group.MapGet("/", [Authorize(Roles = "Admin")] async (IMediator mediator) =>
        {
            var query = new GetAllPaiementsQuery();
            var paiements = await mediator.Send(query);
            return Results.Ok(paiements);
        });

        group.MapGet("/{id}", [Authorize] async (int id, IMediator mediator) =>
        {
            var query = new GetPaiementByIdQuery(id);
            var paiement = await mediator.Send(query);
            return paiement is null ? Results.NotFound() : Results.Ok(paiement);
        });

        group.MapPost("/", [Authorize(Roles = "Patient,Admin")] async (PaiementCreateDto createDto, IMediator mediator) =>
        {
            var command = new ProcessPaiementCommand(createDto);
            var result = await mediator.Send(command);
            if (!result.Success)
                return Results.BadRequest(new { error = result.Error });

            return Results.Created($"/api/paiements/{result.Value!.Id}", result.Value);
        });

        group.MapGet("/rendez-vous/{rendezVousId}", [Authorize] async (int rendezVousId, IMediator mediator) =>
        {
            var query = new GetPaiementsByRendezVousQuery(rendezVousId);
            var paiements = await mediator.Send(query);
            return Results.Ok(paiements);
        });

        group.MapGet("/patient/{patientId}", [Authorize] async (int patientId, IMediator mediator) =>
        {
            var query = new GetPaiementsByPatientQuery(patientId);
            var paiements = await mediator.Send(query);
            return Results.Ok(paiements);
        });
    }
}
