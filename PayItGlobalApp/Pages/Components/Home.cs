using PayItGlobalApp.Models;
using PayItGlobalApp.Resources;
using System;
using MauiReactor;
using MauiReactor.Shapes;
using System.Linq;
using MauiReactor.Animations;
using Microsoft.Maui.Devices;
using Theme = PayItGlobalApp.Resources.Theme;

namespace PayItGlobalApp.Pages.Components;


class HomeMenuState
{
    public double TranslationX { get; set; } = 220;

    public double RotationY { get; set; } = -12;

    public double MarginLeft { get; set; } = -30.0;

    public double MainScale { get; set; } = 1.0;

    public double MainOpacity { get; set; } = 1.0;
}

partial class Home : Component<HomeMenuState>
{
    [Prop]
    private bool _isShown;

    [Prop]
    private Action _onShowOnboarding;

    [Prop]
    private bool _isMovedBack;

    protected override void OnMountedOrPropsChanged()
    {
        InitializeState();
        base.OnMountedOrPropsChanged();
    }

    void InitializeState()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            State.TranslationX = _isShown ? 0 : 220;
            State.MarginLeft = _isShown ? -30 : 0;
        }
        else
        {
            State.TranslationX = _isShown ? 0 : 300;
        }

        State.RotationY = _isShown ? 0.0 : -12;
        State.MainScale = _isMovedBack ? 0.95 : 1.0;
        State.MainOpacity = _isMovedBack ? 0.1 : 1.0;
    }

    public override VisualNode Render()
    {
        return Grid("*", "*",
            Label("Home Page Content")
                .FontSize(24)
                .TextColor(Colors.Black)
                .HCenter()
                .VCenter()
        )
        .BackgroundColor(Theme.Background);
    }

    ImageButton RenderUserButton() =>
        ImageButton("user_white.png")
            .Aspect(Aspect.Center)
            .CornerRadius(18)
            .Shadow(new Shadow().Brush(Theme.ShadowBrush)
                .Opacity(0.1f).Offset(5, 5))
            .HeightRequest(36)
            .WidthRequest(36)
            .HEnd()
            .VStart()
            .Margin(24, 54)
            .BackgroundColor(Colors.White)
            .OnClicked(_onShowOnboarding);

}

class MenuButtonState
{
    public double TranslationX { get; set; } = 0;
}

partial class MenuButton : Component<MenuButtonState>
{
    [Prop]
    private Action _onToggle;

    [Prop]
    private bool _isShown;

    protected override void OnPropsChanged()
    {
        State.TranslationX = _isShown ? 180 : 0;
        base.OnPropsChanged();
    }

    public override VisualNode Render() 
        => Grid("44", "44",
            ContentView(
                RenderButton("menu_black.png", !_isShown)
            )
            .Opacity(!_isShown ? 1.0 : 0.0)
            .WithAnimation(easing: Easing.CubicIn, duration: 300),


            ContentView(
                RenderButton("close_black.png", _isShown)
            )
            .Opacity(_isShown ? 1.0 : 0.0)
            .WithAnimation(easing: Easing.CubicIn, duration: 300)
        )
        .VStart()
        .HStart()
        .Margin(24, 54)
        .TranslationX(State.TranslationX)
        .WithAnimation(easing: Easing.CubicIn, duration: 300);

    ImageButton RenderButton(string image, bool show) 
        => ImageButton(image)
            .Aspect(Aspect.Center)
            .CornerRadius(18)
            .Shadow(new Shadow().Brush(show ? Theme.ShadowBrush : Theme.ShadowDarkBrush)
                .Opacity(0.1f).Offset(5, 5))
            .HeightRequest(36)
            .WidthRequest(36)
            .BackgroundColor(Colors.White)
            .OnClicked(_onToggle);
}