using Bunit;
using SanteSenegal.Web.Layout;
using Xunit;

namespace SanteSenegal.Web.Tests;

public sealed class PublicNavigationTests : TestContext
{
    [Fact]
    public void PublicHeader_RendersOnlyPublicDesktopLinks()
    {
        var header = RenderComponent<PublicHeader>();

        var nav = header.Find("nav[aria-label='Navigation principale']");
        var markup = nav.OuterHtml;

        Assert.Contains("Accueil", markup);
        Assert.Contains("Structures", markup);
        Assert.Contains("Triage", markup);
        Assert.Contains("Urgence", markup);
        Assert.DoesNotContain("Dashboard", markup);
        Assert.DoesNotContain("Dispatch", markup);
        Assert.DoesNotContain("Visualisation", markup);
        Assert.DoesNotContain("Paiements", markup);
    }

    [Fact]
    public void BottomNavigation_RendersMobileNavigationWithExpectedLinks()
    {
        var navigation = RenderComponent<BottomNavigation>();

        var nav = navigation.Find("nav[aria-label='Navigation mobile']");
        var links = nav.QuerySelectorAll("a.bottom-navigation__item");

        Assert.Equal(4, links.Length);
        Assert.Contains(links, link => NormalizeHref(link.GetAttribute("href")) == string.Empty && link.TextContent.Contains("Accueil"));
        Assert.Contains(links, link => NormalizeHref(link.GetAttribute("href")) == "structures" && link.TextContent.Contains("Structures"));
        Assert.Contains(links, link => NormalizeHref(link.GetAttribute("href")) == "triage" && link.TextContent.Contains("Triage"));
        Assert.Contains(links, link => NormalizeHref(link.GetAttribute("href")) == "accident" && link.TextContent.Contains("Urgence"));
    }

    [Fact]
    public void LanguageSelector_TogglesSelectedLanguage()
    {
        var selector = RenderComponent<LanguageSelector>();
        var buttons = selector.FindAll("button");

        Assert.Equal("true", buttons[0].GetAttribute("aria-pressed"));
        Assert.Equal("false", buttons[1].GetAttribute("aria-pressed"));

        buttons[1].Click();
        buttons = selector.FindAll("button");

        Assert.Equal("false", buttons[0].GetAttribute("aria-pressed"));
        Assert.Equal("true", buttons[1].GetAttribute("aria-pressed"));
    }

    private static string NormalizeHref(string? href) => (href ?? string.Empty).Trim('/');
}
