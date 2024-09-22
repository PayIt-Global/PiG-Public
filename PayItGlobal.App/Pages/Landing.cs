using MauiReactor;
using MauiReactor.Animations;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using MauiReactor.Shapes;
using Microsoft.Maui.Devices;
using PayItGlobalApi.App.Models;
using PayItGlobalApi.App.Pages.Components;
using PayItGlobalApi.App.Resources;
using PayItGlobalApi.App.Themes;
using System;
using System.Linq;

namespace PayItGlobalApi.App.Pages;

partial class Landing<TMainMenuState, TSideMenuState> : Component
    where TMainMenuState : IMainMenuState
    where TSideMenuState : ISideMenuState 
{
    private TMainMenuState? _mainMenuState;
    private TSideMenuState? _sideMenuState;
    public TMainMenuState MainMenuState { get => _mainMenuState; set => _mainMenuState = value; }
    public TSideMenuState SideMenuState { get => _sideMenuState; set => _sideMenuState = value; }

    [Prop]
    private bool _isShown;
    [Prop]
    private bool _isMovedBack;

    protected override void OnMountedOrPropsChanged()
    {
        InitializeState();
        //Routing.RegisterRoute<Login>("login");
        base.OnMountedOrPropsChanged();
    }
    void InitializeState()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            MainMenuState.TranslationX = _isShown ? 0 : 220;
            MainMenuState.MarginLeft = _isShown ? -30 : 0;
        }
        else
        {
            MainMenuState.TranslationX = _isShown ? 0 : 300;
        }

        MainMenuState.RotationY = _isShown ? 0.0 : -12;
        MainMenuState.MainScale = _isMovedBack ? 0.95 : 1.0;
        MainMenuState.MainOpacity = _isMovedBack ? 0.1 : 1.0;
    }
    public override VisualNode Render()
    {
        return Border(
            ScrollView(
                Grid("161, 309, 59, *", "*",
                    RenderUserButton(),

                    Label("Courses")
                        .FontAttributes(MauiControls.FontAttributes.Bold)
                        .FontSize(24)
                        .FontFamily("PoppinsBold")
                        .TextColor(Colors.Black)
                        .VEnd(),

                    ScrollView(
                        HStack(spacing: 20,
                            [.. CourseModel.Courses.Select(RenderCourse)]
                        )
                    )
                    .Orientation(ScrollOrientation.Horizontal)
                    .GridRow(1),

                    Label("Recent")
                        .FontAttributes(MauiControls.FontAttributes.Bold)
                        .FontSize(20)
                        .FontFamily("PoppinsBold")
                        .TextColor(Colors.Black)
                        .GridRow(2)
                        .VEnd(),

                    VStack(spacing: 20,
                        [.. CourseModel.CourseSections.Select(RenderCourseSection)]
                    )
                    .Margin(0, 10, 15, 0)
                    .GridRow(3)
                )
            )
            .Orientation(ScrollOrientation.Vertical)
            .OniOS(_ => _.Margin(0, 50, 0, 0))
        )
        .Margin(MainMenuState.MarginLeft, 0, 0, 0)
        .OniOS(_ => _.Margin(MainMenuState.MarginLeft, -50, 0, -50))
        .RotationY(MainMenuState.RotationY)
        .TranslationX(MainMenuState.TranslationX)
        .Padding(-MainMenuState.MarginLeft + 24, 0, 0, 0)
        .WithAnimation(easing: Easing.CubicIn, duration: 300)

        .AnchorX(0.5)
        .AnchorY(0.0)
        .Opacity(MainMenuState.MainOpacity)
        .WithAnimation(easing: ExtendedEasing.InOutBack, duration: 300)

        .StrokeCornerRadius(30, 0, 30, 0)
        .Background(SideMenuTheme.Background)
        ;
    }

    ImageButton RenderUserButton() =>
        ImageButton("user_white.png")
            .Aspect(Aspect.Center)
            .CornerRadius(18)
            .Shadow(new Shadow().Brush(SideMenuTheme.ShadowBrush)
                .Opacity(0.1f).Offset(5, 5))
            .HeightRequest(36)
            .WidthRequest(36)
            .HEnd()
            .VStart()
            .Margin(24, 54)
            .BackgroundColor(Colors.White)
            .OnClicked(OnOpenLoginPage);


    VisualNode RenderCourse(CourseModel model)
    {
        VisualNode RenderAvatar(string image)
            => Image(image)
                .Aspect(Aspect.AspectFit)
                .HeightRequest(44)
                .WidthRequest(44)
                .Clip(new EllipseGeometry().RadiusX(22).RadiusY(22).Center(22, 22))
                ;

        return Border(
            Grid("92, 44, *, 44", "*,44",
                Label(model.Title)
                    .FontAttributes(MauiControls.FontAttributes.Bold)
                    .FontSize(24)
                    .FontFamily("PoppinsBold")
                    .TextColor(Colors.White)
                    .VStart()
                    ,

                Image(model.Image)
                    .GridColumn(1)
                    .VStart(),

                Label(model.SubTitle)
                    .GridRow(1)
                    .TextColor(Colors.White.WithAlpha(0.5f))
                    .FontSize(15)
                    .Margin(0, 6, 0, 0),

                Label(model.Caption.ToUpperInvariant())
                    .GridRow(2)
                    .GridColumnSpan(2)
                    .TextColor(Colors.White.WithAlpha(0.5f))
                    .FontSize(13)
                    .Margin(0, 2, 0, 0)
                    .FontAttributes(MauiControls.FontAttributes.Bold)
                    .VStart(),

                HStack(spacing: -8,
                    RenderAvatar("avatar_4.png"),
                    RenderAvatar("avatar_5.png"),
                    RenderAvatar("avatar_6.png")
                )
                .GridColumnSpan(2)
                .GridRow(3)
            )
        )
        .Padding(30)
        .HeightRequest(309)
        .WidthRequest(260)
        .BackgroundColor(model.Color)
        .StrokeCornerRadius(DeviceInfo.Current.Platform == DevicePlatform.iOS ? 20 : 30)
        .Shadow(new Shadow().Opacity(0.2f).Offset(5, 5).Brush(SideMenuTheme.ShadowBrush));
    }

    VisualNode RenderCourseSection(CourseModel model)
    {
        return Border(
            Grid("*,*", "*,44",
                Label(model.Title)
                    .FontSize(24)
                    .FontFamily("PoppinsBold")
                    .TextColor(Colors.White)
                    ,

                Image(model.Image)
                    .GridColumn(1)
                    .GridRowSpan(2)
                    .VCenter(),

                Rectangle()
                    .VFill()
                    .HEnd()
                    .WidthRequest(1)
                    .GridRowSpan(2)
                    .Margin(15, 5)
                    .Fill(SideMenuTheme.Background2.WithAlpha(0.5f))
                    ,

                Label(model.SubTitle)
                    .GridRow(1)
                    .GridColumnSpan(2)
                    .TextColor(Colors.White)
                    .FontSize(15)
                    .Margin(0, 5, 0, 0)
            )
        )
        .Padding(30, 26)
        .HeightRequest(110)
        .BackgroundColor(model.Color)
        .StrokeCornerRadius(DeviceInfo.Current.Platform == DevicePlatform.iOS ? 15 : 20)
        ;
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
