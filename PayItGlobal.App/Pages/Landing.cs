using MauiReactor;

namespace PayItGlobal.App.Pages;

class Landing : Component
{
    public override VisualNode Render()
    {
        return new Grid("*", "*")
        {
            new Button("Go to Login")
                // Assuming Navigation is a property or method available in your context
                .OnClicked(async () => await Navigation.PushAsync<Login>())
        };
    }
}
