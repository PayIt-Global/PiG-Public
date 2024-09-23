using PayItGlobalApp.Models;
using PayItGlobalApp.Pages.Components;
using System;
using Theme = PayItGlobalApp.Resources.Theme;

namespace PayItGlobalApp.Pages;

class MainPageState
{
    public bool IsSideMenuShown { get; set; }
    public bool ShowOnboarding { get; set; }
    public bool ShowHome { get; set; } = true;
    public NavItem CurrentPage { get; set; } = NavItem.Home;
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    {
        Console.WriteLine($"CurrentPage: {State.CurrentPage}"); // Add this line for debugging

        return ContentPage(
            Grid("*", "*",
                State.CurrentPage switch
                {
                    NavItem.Home => (VisualNode)new Home()
                        .IsShown(!State.IsSideMenuShown)
                        .IsMovedBack(State.ShowOnboarding)
                        .OnShowOnboarding(() => SetState(s => s.ShowOnboarding = true)),
                    NavItem.Teams => new Teams(), // Add this line to switch to Teams
                    NavItem.Keys => new Keys(), // Add this line to switch to Keys
                    NavItem.Reports => new Reports(), // Add this line to switch to Reports
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


