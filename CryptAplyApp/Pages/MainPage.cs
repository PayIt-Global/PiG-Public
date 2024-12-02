using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Models;
using CryptAplyApp.Pages.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using Theme = CryptAplyApp.Resources.Theme;

namespace CryptAplyApp.Pages;

class MainPageState
{
    public bool IsSideMenuShown { get; set; }
    public bool ShowOnboarding { get; set; }
    public bool ShowHome { get; set; } = true;
    public NavItem CurrentPage { get; set; } = NavItem.Home;
    public bool IsAuthenticated { get; set; } = false; // Add this property
}

class MainPage : Component<MainPageState>
{
    private readonly IClientAuthenticationService _authService;

    public MainPage()
    {
        _authService = IPlatformApplication.Current.Services.GetService<IClientAuthenticationService>();
    }

    protected override async void OnMounted()
    {
        base.OnMounted();
        State.IsAuthenticated = await _authService.IsLoggedInAsync();
        Invalidate();
    }

    public override VisualNode Render()
    {
        // Check if the user is authenticated
        if (!State.IsAuthenticated)
        {
            // Show the login component within the main page
            return ContentPage(
                new Login()
                    .OnClose(() => SetState(s => s.IsAuthenticated = true))
                    .Show(true)
            );
        }

        Console.WriteLine($"CurrentPage: {State.CurrentPage}"); // Add this line for debugging

        return ContentPage(
            Grid("*", "*",
                State.CurrentPage switch
                {
                    NavItem.Home => (VisualNode)new Home()
                        .IsShown(!State.IsSideMenuShown)
                        .IsMovedBack(State.ShowOnboarding)
                        .OnShowOnboarding(() => SetState(s => s.ShowOnboarding = true)),
                    NavItem.Teams => new Teams(),
                    NavItem.Keys => new Keys(),
                    NavItem.Reports => new Reports(),
                    NavItem.Help => new Help(),
                    _ => new Home() // Default to Home
                },

                new SideMenu()
                    .IsShown(State.IsSideMenuShown),

                new MenuButton()
                    .IsShown(State.IsSideMenuShown)
                    .OnToggle(() => SetState(s => s.IsSideMenuShown = !s.IsSideMenuShown)),

                new Onboarding()
                    .Show(State.ShowOnboarding)
                    .OnClose(() => SetState(s => s.ShowOnboarding = false)),

                new NavBar()
                    .OnHelpSelected(() => SetState(s => s.CurrentPage = NavItem.Help)) // Update this line
                    .OnHomeSelected(() => SetState(s => s.CurrentPage = NavItem.Home)) // Update this line
                    .OnTeamsSelected(() => SetState(s => s.CurrentPage = NavItem.Teams)) // Add this line
                    .OnKeysSelected(() => SetState(s => s.CurrentPage = NavItem.Keys)) // Add this line
                    .OnReportsSelected(() => SetState(s => s.CurrentPage = NavItem.Reports)) // Add this line
                    .Show(!State.IsSideMenuShown && !State.ShowOnboarding)
            )
        )
        .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false)
        .BackgroundColor(Theme.Background2);
    }
}
