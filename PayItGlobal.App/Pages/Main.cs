using System;
using System.Linq;
using PayItGlobal.App.Resources.Styles;
using MauiReactor;
using MauiReactor.Canvas;
using Microsoft.Extensions.DependencyInjection;
using PayItGlobal.Application.Interfaces;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages;

enum PageEnum
{
    Home,
    Landing
}

class MainPageState
{
    public PageEnum CurrentPage { get; set; }
    public bool IsAuthenticated { get; set; } = false; // Assume this will be set based on actual authentication status
}

class Main : Component<MainPageState>
{
    // Placeholder for actual authentication check logic
    private bool CheckAuthentication()
    {
        // Implement your authentication check logic here
        // For example, return _authenticationService.IsAuthenticated;
        return false; // Placeholder return value
    }

    protected override void OnMounted()
    {
        // Set the initial page based on authentication status
        State.CurrentPage = CheckAuthentication() ? PageEnum.Home : PageEnum.Landing;
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new NavigationPage
        {
            new ContentPage
            {
                new Grid("*", "*")
                {
                    RenderPage(),
                    RenderTabBar()
                }
            }
            .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false)
            .BackgroundColor(ThemeBrushes.Background)
        };
    }

    VisualNode RenderPage() => State.CurrentPage switch
    {
        PageEnum.Home => new Home(),
        PageEnum.Landing => new Landing(),
        _ => throw new NotImplementedException(),
    };

    VisualNode RenderTabBar()
    {
        ImageButton createButton(PageEnum page, int column) =>
            new ImageButton()
                .Aspect(Aspect.Center)
                .Source(() => State.CurrentPage != page ? $"{page.ToString().ToLowerInvariant()}.png" : $"{page.ToString().ToLowerInvariant()}_on.png")
                .GridColumn(column)
                .OnClicked(() => SetState(s => s.CurrentPage = page))
                .Margin(State.CurrentPage == page ? new Thickness(0, 0, 0, 10) : new Thickness())
                .WithAnimation(Easing.BounceOut, 300)
                ;

        return new Grid("*", "*")
        {
            new CanvasView()
            {
                new DropShadow
                {
                    new Box()
                        .Margin(0,20,0,0)
                        .CornerRadius(24,24,0,0)
                        .BackgroundColor (ThemeBrushes.White)
                }
                .Color(ThemeBrushes.DarkShadow)
                .Size(0, -8)
                .Blur(32),

                new Row("* * * * *")
                {
                    Enum.GetValues(typeof(PageEnum))
                        .Cast<PageEnum>()
                        .Select(page =>
                            new Align()
                            {
                                new Ellipse()
                                    .FillColor(ThemeBrushes.Purple10)
                            }
                            .IsVisible(State.CurrentPage == page)
                            .Width(4)
                            .Height(4)
                            .VEnd()
                            .HCenter()
                            .Margin(16)
                        )
                        .ToArray()
                }

            }
            .BackgroundColor(Colors.Transparent)
            .GridRow(1),

            new Grid("*", "* * * * *")
            {
                createButton(PageEnum.Home, 0),
                createButton(PageEnum.Landing , 1)
            }
            .Padding(0,20,0,0)
        }
        .VEnd()
        .HeightRequest(92);
    }
}


