using MauiReactor;

namespace PayItGlobal.App.Pages;

partial class LandingPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage
        {
           
                new Button("Go to Login")
                    .OnClicked(async () => await Navigation.PushAsync<LoginPage>())

        }
        .Title("Welcome");
    }
}
