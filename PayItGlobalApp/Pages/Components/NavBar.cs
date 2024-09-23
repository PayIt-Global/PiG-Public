using MauiReactor.Animations;
using MauiReactor.Canvas;
using MauiReactor.Shapes;
using PayItGlobalApp.Models;
using System;

namespace PayItGlobalApp.Pages.Components;

class NavBarState
{
    public NavItem SelectedItem { get; set; } = NavItem.Home;
    public double TranslationY { get; set; }
}

partial class NavBar : Component<NavBarState>
{
    [Prop]
    private bool _show;
    [Prop]
    private Action _onHelpSelected;
    [Prop]
    private Action _onHomeSelected;
    [Prop]
    private Action _onReportsSelected;
    [Prop]
    private Action _onTeamsSelected;
    [Prop]
    private Action _onKeysSelected;

    protected override void OnMountedOrPropsChanged()
    {
        State.TranslationY = _show ? 0 : 150;
        base.OnMountedOrPropsChanged();
    }

    public override VisualNode Render()
    {
        return new CanvasView()//("59", "*")
        {
            new Box()
            {
                new Group()
                {
                    new Box()
                        .Background(new MauiControls.LinearGradientBrush(
                            new MauiControls.GradientStopCollection
                            {
                                new MauiControls.GradientStop(Colors.Transparent, 0.0f),
                                new MauiControls.GradientStop(Colors.White.WithAlpha(0.3f), 1.0f),
                            }, new Point(0.5,0.0), new Point(0.5, 1.0)))
                        .BorderColor(Colors.White.WithAlpha(0.5f))
                        .CornerRadius(22),

                    new Box
                    {
                        new Row()
                        {
                            new NavBarButtonIcon()
                                .Icon("home_img.png")
                                .IsSelected(State.SelectedItem == NavItem.Home)
                                .OnSelected(() =>
                                {
                                    SetState(s => s.SelectedItem = NavItem.Home);
                                    _onHomeSelected?.Invoke(); // Invoke the callback
                                }),
                            new NavBarButtonIcon()
                                .Icon("team_img.png")
                                .IsSelected(State.SelectedItem == NavItem.Teams)
                                .OnSelected(() =>
                                {
                                    SetState(s => s.SelectedItem = NavItem.Teams);
                                    _onTeamsSelected?.Invoke(); // Invoke the callback
                                }),
                            new NavBarButtonIcon()
                                .Icon("key_img.png")
                                .IsSelected(State.SelectedItem == NavItem.Keys)
                                .OnSelected(() =>
                                {
                                    SetState(s => s.SelectedItem = NavItem.Keys);
                                    _onKeysSelected?.Invoke(); // Invoke the callback
                                }),
                            new NavBarButtonIcon()
                                .Icon("reports_img.png") // Update the icon name
                                .IsSelected(State.SelectedItem == NavItem.Reports)
                                .OnSelected(() =>
                                {
                                    SetState(s => s.SelectedItem = NavItem.Reports);
                                    _onReportsSelected?.Invoke(); // Invoke the callback
                                }),
                            new NavBarButtonIcon()
                                .Icon("help_img.png")
                                .IsSelected(State.SelectedItem == NavItem.Help)
                                .OnSelected(() =>
                                {
                                    SetState(s => s.SelectedItem = NavItem.Help);
                                    _onHelpSelected?.Invoke(); // Invoke the callback
                                }),

                        }
                    }
                    .Padding(22, 7)
                }
            }
            .BackgroundColor(Color.FromRgba(24, 34, 60, 256))            
            .CornerRadius(22)

     
        }
        .Margin(35, 0, 35, 30)
        .HeightRequest(59)
        .VEnd()
        .Shadow(new Shadow().Brush(Colors.White).Opacity(0.2f).Radius(200).Offset(0.0, 10.0))
        .TranslationY(State.TranslationY)
        .WithAnimation(ExtendedEasing.InOutBack, duration: 400)
        .BackgroundColor(Colors.Transparent)
        ;

    }
}

class NavBarButtonIconState
{
    public float SelectionScaleX { get; set; }
}

partial class NavBarButtonIcon : Component<NavBarButtonIconState>
{
    [Prop]
    private string _icon;

    [Prop]
    private bool _isSelected;

    [Prop]
    private Action _onSelected;

    protected override void OnMountedOrPropsChanged()
    {
        State.SelectionScaleX = _isSelected ? 1.0f : 0.0f;
        base.OnMountedOrPropsChanged();
    }    

    public override VisualNode Render()
    {
        return new PointInteractionHandler
        {
            new Align
            {
                new Column("11, 24") //new Grid("11, 24", "46")
                {
                    new Align()
                    {
                        new Box()
                            .BackgroundColor(Color.FromArgb("#81B4FF"))
                            //.BackgroundColor(Colors.Red)
                            .CornerRadius(3)
                    }
                    .VCenter()
                    .HCenter()
                    .Height(5)
                    .Width(20)
                    .AnchorX(0.5f)
                    .ScaleX(State.SelectionScaleX) 
                    .WithAnimation(duration: 100),

                    new AnimatedIcon()
                        .Icon(_icon)
                        .IsSelected(_isSelected)
                        ,
                }
            }
            .Width(46)
        }
        .OnTap(_onSelected)
        ;
    }
}
