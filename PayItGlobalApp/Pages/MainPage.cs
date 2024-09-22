using PayItGlobalApp.Pages.Components;
using System;
using Theme = PayItGlobalApp.Resources.Theme;

namespace PayItGlobalApp.Pages;

class MainPageState
{
    public bool IsSideMenuShown { get; set; }
    public bool ShowOnboarding { get; set; }
    public bool ShowHome { get; set; } = true;
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    {
        Console.WriteLine($"ShowHome: {State.ShowHome}"); // Add this line for debugging

        return ContentPage(
            Grid("*", "*",
                State.ShowHome
                    ? (VisualNode)new Home()
                        .IsShown(!State.IsSideMenuShown)
                        .IsMovedBack(State.ShowOnboarding)
                        .OnShowOnboarding(() => SetState(s => s.ShowOnboarding = true))
                    : new Help(), // Ensure this line is present to switch to Help

                new SideMenu()
                    .IsShown(State.IsSideMenuShown),

                new MenuButton()
                    .IsShown(State.IsSideMenuShown)
                    .OnToggle(() => SetState(s => s.IsSideMenuShown = !s.IsSideMenuShown)),

                new Onboarding()
                    .Show(State.ShowOnboarding)
                    .OnClose(() => SetState(s => s.ShowOnboarding = false)),

                new NavBar()
                    .OnHelpSelected(() => SetState(s => s.ShowHome = false)) // Ensure this line is present
                    .Show(!State.IsSideMenuShown && !State.ShowOnboarding)
            )
        )
        .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false)
        .BackgroundColor(Theme.Background2);
    }
}

