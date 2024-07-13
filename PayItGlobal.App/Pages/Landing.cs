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
        var currentTheme = ThemeManager.CurrentTheme; // Access the current theme


        return new Grid("268, *, 92", "*")
        {
            RenderTopPanel(currentTheme)
        }
        .Margin(0, 0, 0, 88);
    }

    VisualNode RenderTopPanel(IThemeColors currentTheme)
    {
        return new Grid("40,40", "*")
        {
            new Label("label1")
                .Text("Landing")
                .FontSize(24)
                .TextColor(currentTheme.OnBackground)
                .GridRow(0)
                .HorizontalTextAlignment(TextAlignment.Center)
                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold),

            new Button("button1")
            .BorderColor(currentTheme.Secondary)
            .BackgroundColor(currentTheme.SecondaryContainer)
            .FontSize(24)
            .GridRow(1)
            .HCenter()
            .HeightRequest(50)
            .Padding(10)
            .Text("the button")
            .TextColor(currentTheme.OnSecondary)
            .VEnd()
            .CornerRadius(10)
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
