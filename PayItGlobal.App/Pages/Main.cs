using MauiReactor;
using MauiReactor.Canvas;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;
using PayItGlobal.App.Resources.Styles;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Application.Services;
using PayItGlobal.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages;

enum PageEnum
{
    Home,
    Landing,
    Login
}

class MainPageState
{
    public PageEnum CurrentPage { get; set; }
    public bool IsAuthenticated { get; set; } = false; // Assume this will be set based on actual authentication status
}

partial class Main : Component<MainPageState>
{
    [Inject]
    IClientAuthenticationService? _authService;
    private CancellationTokenSource? _cts;
    private async Task<bool> CheckAuthentication()
    {
        if (_authService != null)
        {
            return await _authService.IsLoggedInAsync();
        }
        return false;
    }
    private async void CheckAuthenticationAsync()
    {
        try
        {
            var isAuthenticated = await CheckAuthentication();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_cts.IsCancellationRequested)
                {
                    State.CurrentPage = isAuthenticated ? PageEnum.Home : PageEnum.Landing;
                }
            });
        }
        catch (Exception ex)
        {
            // Handle exception, possibly log it or show an error message
            Console.WriteLine($"Error during authentication check: {ex.Message}");
        }
    }

    protected override void OnMounted()
    {
        _cts = new CancellationTokenSource();
        CheckAuthenticationAsync();
        base.OnMounted();
    }

    public void Cleanup()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
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
        PageEnum.Login => new Login(),
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


