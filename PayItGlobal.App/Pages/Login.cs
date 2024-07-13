using MauiReactor;
using MauiReactor.Canvas;
using Microsoft.Extensions.DependencyInjection;
using PayItGlobal.Application.Interfaces;
using System;
using PayItGlobal.Application.Infrastructure.Utilities;
using PayItGlobal.App.Themes;

namespace PayItGlobal.App.Pages;

class LoginPageState
{
    public string Username { get; set; }
    public string Password { get; set; }
}

class LoginPageProps
{
    public Action? OnLogged { get; set; }
}

class Login : Component<LoginPageState, LoginPageProps>
{
    public override VisualNode Render()
    {
        var currentTheme = ThemeManager.CurrentTheme; // Assuming ThemeManager is accessible

        return new ContentPage
        {
            new Grid("268, *, 92", "*")
            {
                RenderTopPanel(currentTheme)
            }
            .Margin(20) // Adjusted margin for overall padding
            .BackgroundColor(currentTheme.Background) // Use theme background color
        };
    }

    VisualNode RenderTopPanel(IThemeColors currentTheme)
    {
        return Border(
            Grid("Auto, Auto, Auto", "*", // Three rows, each sized according to their content
                    new Entry()
                    .BackgroundColor(currentTheme.Surface)
                    .PlaceholderColor(currentTheme.TertiaryContainer)
                    .Placeholder("Username")
                    .TextColor(currentTheme.Tertiary)
                    .GridRow(0)
                    .OnTextChanged((s, e) => SetState(_ => _.Username = e.NewTextValue))
                    .Margin(new Thickness(16, 8)),

                new Entry()
                    .BackgroundColor(currentTheme.Surface)
                    .PlaceholderColor(currentTheme.TertiaryContainer)
                    .Placeholder("Password")
                    .TextColor(currentTheme.Tertiary)
                    .GridRow(1)
                    .OnTextChanged((s, e) => SetState(_ => _.Username = e.NewTextValue))
                    .Margin(new Thickness(16, 8)),

                new Button("Login")
                    .IsEnabled(!string.IsNullOrWhiteSpace(State.Username) && !string.IsNullOrWhiteSpace(State.Password))
                    .OnClicked(OnLogin)
                    .BackgroundColor(currentTheme.Tertiary)
                    .TextColor(currentTheme.OnTertiary)
                    .CornerRadius(10)
                    .GridRow(2)
                    .Margin(new Thickness(16, 8))
                    )
            )
            .Padding(30, 26)
            .HeightRequest(220) // Adjust height as needed for the Border
            .WidthRequest(250) // Adjust width as needed for the Border
            .BackgroundColor(currentTheme.PrimaryContainer) // Set your desired background color for the Border
            .StrokeCornerRadius(20) // Set corner radius for rounded corners of the Border
            .Shadow(new Shadow().Opacity(0.2f).Offset(5, 5).Brush(currentTheme.Shadow)); // Shadow for the Border
    }



    private async void OnLogin()
    {
        try
        {
            // Show loading indicator
            string username = State.Username;
            string password = State.Password;
            string userIpAddress = await NetworkUtilities.GetUserIpAddressAsync();
            var authService = Services.GetRequiredService<IClientAuthenticationService>();
            bool isLoggedIn = await authService.LogInAsync(username, password, userIpAddress);

            if (isLoggedIn)
            {
                NavigateToHomePage();
            }
            else
            {
                // Show error message: "Login failed. Please try again."
                //ShowMessage("Login failed. Please try again.");
            }
        }
        catch (Exception ex)
        {
            // Hide loading indicator
            // Log the exception
            // Show error message: "An error occurred. Please try again later."
        }
    }

    async void NavigateToHomePage()
    {
        if (Navigation != null)
        {
            await Navigation.PushAsync<Home, HomePageProps>(props =>
            {
                props.RenderAsPage = true;
            });
        }
        else
        {
            throw new Exception("Navigation is null");
        }
    }

    void ShowMessage(string message)
    {
        // Show a message to the user
        // This could involve using a dialog service or a temporary notification component
    }
}
