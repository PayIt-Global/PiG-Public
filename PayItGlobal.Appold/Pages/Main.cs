using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Parameters;
using Microsoft.Maui.Storage;
using PayItGlobal.Appold.Resources.Styles;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.DTOs.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace PayItGlobal.Appold.Pages;

enum PageEnum
{
    Home,
    Reports,
    Accounting,   
    TakePayment,
    Invoices,
    Landing
}

class MainPageState
{
    public PageEnum CurrentPage { get; set; }
    
    public bool Loading { get; set; }
}
class MainState
{
    public UserViewModel? CurrentUser { get; set; }
}
partial class MainPage : Component<MainPageState>
{
    private readonly IParameter<MainState> _mainState;

    //public MainPage()
    //{
    //    _mainState = CreateParameter<MainState>();
    //}

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
    protected override async void OnMounted()
    {
        State.Loading = true;

        _mainState.Set(s => s.CurrentUser = Preferences.Default.GetFromJson<UserViewModel?>("current_user", null));

        if (_mainState.Value.CurrentUser != null)
        {

        }

        SetState(s => s.Loading = false);

        base.OnMounted();
    }



    public override VisualNode Render()
    {
        if (State.Loading)
        {
            return ContentPage(
                ActivityIndicator()
                    .IsVisible(true)
                    .IsRunning(true)
                    .HCenter()
                    .VCenter()
            )
            .BackgroundColor(PayItGlobal.Appold.Resources.Styles.Theme.Current.Background);
        }

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
        PageEnum.Home => new HomePage(),
        PageEnum.TakePayment => new TakePaymentPage(),
        PageEnum.Reports => new ReportsPage(),
        PageEnum.Accounting => new AccountingPage(),
        PageEnum.Invoices => new InvoicesPage(),
        PageEnum.Landing => new LandingPage(),
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
                createButton(PageEnum.Reports, 1),
                createButton(PageEnum.Accounting, 2),
                createButton(PageEnum.TakePayment, 3),
                createButton(PageEnum.Invoices, 4)
            }
            .Padding(0,20,0,0)
        }
        .VEnd()
        .HeightRequest(92);
    }
}


