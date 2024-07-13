using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using PayItGlobal.App.Themes;
using System;

namespace PayItGlobal.App.Pages;

class LandingPageState
{

}

partial class Landing : Component
{
    protected override void OnMountedOrPropsChanged()
    {
        //Routing.RegisterRoute<Login>("login");
        base.OnMountedOrPropsChanged();
    }
    public override VisualNode Render()
    {
        return new Grid("268, *, 92", "*")
        {
            RenderTopPanel()
        }
        .Margin(0, 0, 0, 88);
    }

    VisualNode RenderTopPanel()
    {
        var currentTheme = ThemeManager.CurrentTheme; // Access the current theme

        return new Grid("40,40", "*")
        {
            new Label("label1")
                .Text("Landing")
                .FontSize(24)
                .TextColor(currentTheme.OnBackground)
                .GridRow(0)
                .HorizontalTextAlignment(TextAlignment.Center),

            new Button("button1")
            .BorderColor(currentTheme.Secondary)
            .FontSize(24)
            .GridRow(1)
            .HCenter()
            .HeightRequest(30)
            .Padding(3)
            .Text("the button")
            .TextColor(currentTheme.OnSecondary)
            .VEnd()
            .OnClicked(OnOpenLoginPage)
        };
    }
    private async void OnOpenLoginPage()
    {
        if (Navigation != null)
        {            
            await Navigation.PushAsync<Login>();
        }
        else
        {
            throw new Exception("Navigation is null");
        }
    }
}
