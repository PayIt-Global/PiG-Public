using MauiReactor;

namespace PayItGlobal.Appold.Pages;

partial class Landing : Component
{
    public override VisualNode Render()
    {
        return new ContentPage
        {
           
                new Button("Go to Login")
                    .OnClicked(async () => await Navigation.PushAsync<Login>())

        }
        .Title("Welcome");
    }
}
