using MauiReactor;

namespace PayItGlobal.App.Pages;

class LoginPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("LoginPage")
        {
            new Button("Goto back")
                .HCenter()
                .VCenter()
            .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(".."))
        };
    }
}
