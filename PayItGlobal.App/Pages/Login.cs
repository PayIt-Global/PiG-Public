using MauiReactor;
using MauiReactor.Canvas;
using Microsoft.Extensions.DependencyInjection;
using PayItGlobal.Application.Interfaces;
using System;

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
        return new ContentPage
        {
             new Grid("268, *, 92", "*")
            {
                RenderTopPanel()
            }
            .Margin(0, 0, 0, 88)
        };
    }

    VisualNode RenderTopPanel()
    {
        return new StackLayout()
            {
                new Entry()
                    .Placeholder("Username")
                    .OnTextChanged((s,e)=> SetState(_ => _.Username = e.NewTextValue)),
                new Entry()
                    .Placeholder("Password")
                    .OnTextChanged((s,e)=> SetState(_ => _.Password = e.NewTextValue)),
                new Button("Login")
                    .IsEnabled(!string.IsNullOrWhiteSpace(State.Username) && !string.IsNullOrWhiteSpace(State.Password))
                    .OnClicked(OnLogin)
            }
        .VCenter()
        .HCenter();
    }
    private async void OnLogin()
    {
        try
        {
            // Show loading indicator
            string username = State.Username;
            string password = State.Password;
            var authService = Services.GetRequiredService<IClientAuthenticationService>();
            bool isLoggedIn = await authService.LogInAsync(username, password);

            if (isLoggedIn)
            {
                // Hide loading indicator
                // Navigate to the next page or show success message
            }
            else
            {
                // Hide loading indicator
                // Show error message: "Login failed. Please try again."
            }
        }
        catch (Exception ex)
        {
            // Hide loading indicator
            // Log the exception
            // Show error message: "An error occurred. Please try again later."
        }
    }

}
