using MauiReactor;
using PayItGlobal.App.Resources;
using System;

namespace PayItGlobal.App.Pages.Components
{

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
                .Shadow(new Shadow().Brush(show ? SideMenuTheme.ShadowBrush : SideMenuTheme.ShadowDarkBrush)
                    .Opacity(0.1f).Offset(5, 5))
                .HeightRequest(36)
                .WidthRequest(36)
                .BackgroundColor(Colors.White)
                .OnClicked(_onToggle);
    }
}
