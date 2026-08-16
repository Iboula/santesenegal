using Bunit;
using Microsoft.AspNetCore.Components;
using SanteSenegal.Web.Components.Badges;
using SanteSenegal.Web.Components.Buttons;
using SanteSenegal.Web.Components.Cards;
using SanteSenegal.Web.Components.Feedback;
using Xunit;

namespace SanteSenegal.Web.Tests;

public sealed class DesignSystemTests : TestContext
{
    [Fact]
    public void PrimaryButton_RendersAccessibleButton()
    {
        var button = RenderComponent<PrimaryButton>(parameters => parameters
            .Add(p => p.AriaLabel, "Confirmer le rendez-vous")
            .AddChildContent("Confirmer"));

        var element = button.Find("button.ds-button--primary");

        Assert.Equal("button", element.GetAttribute("type"));
        Assert.Equal("Confirmer le rendez-vous", element.GetAttribute("aria-label"));
        Assert.Contains("Confirmer", element.TextContent);
    }

    [Fact]
    public void SecondaryButton_RendersLinkWhenHrefIsProvided()
    {
        var button = RenderComponent<SecondaryButton>(parameters => parameters
            .Add(p => p.Href, "/structures")
            .Add(p => p.AriaLabel, "Voir les structures")
            .AddChildContent("Structures"));

        var element = button.Find("a.ds-button--secondary");

        Assert.Equal("structures", element.GetAttribute("href")?.Trim('/'));
        Assert.Equal("Voir les structures", element.GetAttribute("aria-label"));
    }

    [Theory]
    [InlineData(StatusBadgeType.Ouvert, "Ouvert", "ds-badge--open")]
    [InlineData(StatusBadgeType.Ferme, "Fermé", "ds-badge--closed")]
    [InlineData(StatusBadgeType.Disponible, "Disponible", "ds-badge--available")]
    [InlineData(StatusBadgeType.Occupe, "Occupé", "ds-badge--busy")]
    [InlineData(StatusBadgeType.Urgence, "Urgence", "ds-badge--emergency")]
    public void StatusBadge_RendersSupportedStatuses(StatusBadgeType status, string label, string cssClass)
    {
        var badge = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, status));

        var element = badge.Find("span.ds-badge");

        Assert.Contains(label, element.TextContent);
        Assert.Contains(cssClass, element.ClassName);
        Assert.Equal($"Statut: {label}", element.GetAttribute("aria-label"));
    }

    [Fact]
    public void FacilityCard_RendersFacilityDetailsAndActions()
    {
        RenderFragment actions = builder =>
        {
            builder.OpenComponent<PrimaryButton>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
            {
                content.AddContent(2, "Choisir");
            }));
            builder.CloseComponent();
        };

        var card = RenderComponent<FacilityCard>(parameters => parameters
            .Add(p => p.Title, "Centre de santé")
            .Add(p => p.Subtitle, "Plateau")
            .Add(p => p.Availability, StatusBadgeType.Disponible)
            .Add(p => p.Distance, "1,2 km")
            .Add(p => p.WaitingTime, "15 min")
            .Add(p => p.Actions, actions));

        Assert.Contains("Centre de santé", card.Markup);
        Assert.Contains("Plateau", card.Markup);
        Assert.Contains("Disponible", card.Markup);
        Assert.Contains("1,2 km", card.Markup);
        Assert.Contains("15 min", card.Markup);
        Assert.Contains("Actions structure", card.Markup);
        Assert.Contains("Choisir", card.Markup);
    }

    [Fact]
    public void LoadingState_RendersPoliteStatus()
    {
        var state = RenderComponent<LoadingState>(parameters => parameters
            .Add(p => p.Message, "Chargement des structures"));

        var element = state.Find("[role='status']");

        Assert.Equal("polite", element.GetAttribute("aria-live"));
        Assert.Contains("Chargement des structures", element.TextContent);
    }

    [Fact]
    public void EmptyState_RendersStatusWithCustomContent()
    {
        var state = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.Title, "Aucune structure")
            .Add(p => p.Message, "Essayez une autre recherche."));

        Assert.NotNull(state.Find("[role='status']"));
        Assert.Contains("Aucune structure", state.Markup);
        Assert.Contains("Essayez une autre recherche.", state.Markup);
    }

    [Fact]
    public void ErrorState_RendersAlert()
    {
        var state = RenderComponent<ErrorState>(parameters => parameters
            .Add(p => p.Title, "Service indisponible")
            .Add(p => p.Message, "Réessayez plus tard."));

        Assert.NotNull(state.Find("[role='alert']"));
        Assert.Contains("Service indisponible", state.Markup);
        Assert.Contains("Réessayez plus tard.", state.Markup);
    }
}
