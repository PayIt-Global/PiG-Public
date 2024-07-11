using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PayItGlobal.App.Resources.Styles;
using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using Microsoft.Maui.Devices;
using Android.Mtp;

namespace PayItGlobal.App.Pages;

class LandingPageState
{

}

partial class Landing : Component
{
    [Prop]
    private Action? _onLogged;
    protected override void OnMountedOrPropsChanged()
    {
        Routing.RegisterRoute<Login>("login");
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
        return new Grid("40,40", "*")
        {
            new Label("label1")
                .Text("Landing")
                .FontSize(24)
                .TextColor(ThemeBrushes.Dark)
                .GridRow(0)
                .HorizontalTextAlignment(TextAlignment.Center),

            new Button("button1")
            .BorderColor(ThemeBrushes.Purple30)
            .FontSize(24)
            .GridRow(1)
            .HCenter()
            .HeightRequest(30)
            .Padding(3)
            .Text("the button")
            .TextColor(ThemeBrushes.Grey100)
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
