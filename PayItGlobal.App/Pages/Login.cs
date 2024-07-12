using MauiReactor;
using MauiReactor.Canvas;
using Microsoft.Extensions.DependencyInjection;
using PayItGlobal.Application.Interfaces;
using System;
using PayItGlobal.Application.Infrastructure.Utilities;

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
                props.RenderAsPage = true; // Or true, based on your logic
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
