using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayItGlobalApi.App.Themes;

namespace PayItGlobalApi.App.Pages;

class EventDetailsPageState
{
    public double ImageTransaleX { get; set; }
    public double ImageTransaleY { get; set; }
    public double ImageHeight { get; set; }
    public double ImageOpacity { get; set; }
    public double MainPanelX { get; set; }
}

class EventDetailsPageProps
{
    public Rect SourceRect { get; set; }
}

class EventDetails : Component<EventDetailsPageState, EventDetailsPageProps>
{
    private IThemeColors CurrentTheme => ThemeManager.CurrentTheme;

    protected override void OnMounted()
    {
        State.ImageTransaleX = Props.SourceRect.X;
        State.ImageTransaleY = Props.SourceRect.Y;
        State.ImageHeight = Props.SourceRect.Height;
        State.MainPanelX = 700;

        MauiControls.Application.Current?.Dispatcher.Dispatch(new Action(() => SetState(s =>
        {
            s.ImageTransaleX = 0;
            s.ImageTransaleY = 0;
            s.ImageHeight = 345;
            s.ImageOpacity = 1;
            s.MainPanelX = 313;
        })));

        base.OnMounted();
    }


    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("*, 104", "*")
            {
                RenderTopPanel(),
                RenderMainPanel(),
                RenderBottomPanel()
            }
        }
        .BackgroundColor(CurrentTheme.Background)
        .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false);
    }

    VisualNode[] RenderTopPanel()
        => new VisualNode[]{
            new CanvasView
                {
                    new ClipRectangle()
                    {

                    }
                    .CornerRadius(0,0,8,8),
                }
                .Margin(State.ImageTransaleX, State.ImageTransaleY, State.ImageTransaleX, 0)
                .HeightRequest(State.ImageHeight)
                .Opacity(State.ImageOpacity)
                .WithAnimation(duration: 300)
                .BackgroundColor(Colors.Transparent)
                .VStart(),

                new ImageButton("back_white.png")
                    .Margin(16, 38)
                    .HStart()
                    .VStart()
                    .HeightRequest(32)
                    .WidthRequest(32)
                    .OnClicked(OnBack),

                new ImageButton("share.png")
                    .Margin(64, 42)
                    .HEnd()
                    .VStart()
                    .HeightRequest(24)
                    .WidthRequest(24),

                new ImageButton("fav_white.png")
                    .Margin(16, 42)
                    .HEnd()
                    .VStart()
                    .HeightRequest(24)
                    .WidthRequest(24)
            };

    VisualNode RenderMainPanel()
        => new CanvasView
        {
            new Align
            {
                new PointInteractionHandler
                {
                }
                .OnTap(()=>SetState(s => s.MainPanelX = s.MainPanelX == 72 ? 313 : 72))
            }
            .Height(64)
            .VStart(),

            new Box
            {
                new Column("40, 48, 64, 88, 52, 186")
                {
                    new Align
                    {
                        new Box()
                            .BackgroundColor(CurrentTheme.Surface)
                    }
                    .Height(4)
                    .Width(64)
                    .HCenter()
                    .VStart(),

                    new Text("Sit amet odio nisi leo viverra sed a vel blandit adipiscing")
                        .FontSize(24)
                        .FontWeight(FontWeights.Bold)
                        .FontColor(CurrentTheme.Surface),

                    new Row("32, *, 75")
                    {
                        new Picture($"PayItGlobal.App.Resources.Images.photo5.png"),

                        new Column
                        {
                            new Text("Floys Miles")
                                .FontSize(14)
                                .FontColor(CurrentTheme.OnBackground)
                                .FontWeight(FontWeights.Bold)
                                .VerticalAlignment(VerticalAlignment.Center),
                            new Text(DateTime.Today.ToShortDateString())
                                .FontColor(CurrentTheme.OnSurface)
                                .VerticalAlignment(VerticalAlignment.Center),
                        }
                        .Margin(8,4.5f,0,4.5f),

                        new Box
                        {
                            new Text(DateTime.Now.ToShortTimeString())
                                .FontWeight(FontWeights.Bold)
                                .VerticalAlignment (VerticalAlignment.Center)
                                .HorizontalAlignment(HorizontalAlignment.Center),
                        }
                        .BackgroundColor(CurrentTheme.SurfaceVariant)
                        .CornerRadius(8)
                        .Margin(0,4.5f),

                    }
                    .Margin(0,24,0,0),

                    new Box()
                    {
                        new Row("48, *, 32")
                        {
                            new Box()
                            {
                                new Align
                                {
                                    new Picture($"PayItGlobal.App.Resources.Images.event_calendar.png")
                                }
                                .Height(24)
                                .Width(24)
                                .VCenter()
                                .HCenter()
                            }
                            .BackgroundColor (CurrentTheme.SurfaceVariant)
                            .CornerRadius(16),

                            new Column
                            {
                                new Text(DateTime.Today.AddDays(2).ToShortDateString())
                                    .FontSize(14)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .FontColor(CurrentTheme.OnSurface),
                                new Text("10:00 am - 11:30 am")
                                    .FontSize(14)
                                    .FontWeight (FontWeights.Bold)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .FontColor(CurrentTheme.OnBackground)
                            }
                            .Margin(16,6,0,6),

                            new Align
                            {
                                new Picture($"PayItGlobal.App.Resources.Images.chevron_right.png")
                            }
                            .Width(16)
                            .Height(16)
                            .VCenter()
                            .HCenter()
                        }
                        .Margin(8)
                    }
                    .Margin(0, 24, 0, 0)
                    .BackgroundColor(CurrentTheme.Surface)
                    .CornerRadius(16),


                    new Row("20, *, 140")
                    {
                        new Text("32")
                            .FontSize(16)
                            .FontColor(CurrentTheme.OnBackground)
                            .VerticalAlignment(VerticalAlignment.Center),

                        new Text("people are going")
                            .FontColor(CurrentTheme.OnSurface)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .FontSize(14),

                        new Align
                        {
                            new Group
                            {
                                new Align
                                {
                                    new Picture($"PayItGlobal.App.Resources.Images.photo1_circle.png")
                                }
                                .HEnd()
                                .Width(32)
                                .Margin(0,0,22+22+22+22,0),

                                new Align
                                {
                                    new Picture($"PayItGlobal.App.Resources.Images.photo2_circle.png")
                                }
                                .HEnd()
                                .Width(32)
                                .Margin(0,0,22+22+22,0),

                                new Align
                                {
                                    new Picture($"PayItGlobal.App.Resources.Images.photo3_circle.png")
                                }
                                .HEnd()
                                .Width(32)
                                .Margin(0,0,22+22,0),

                                new Align
                                {
                                    new Picture($"PayItGlobal.App.Resources.Images.photo4_circle.png")
                                }
                                .HEnd()
                                .Width(32)
                                .Margin(0,0,22,0),

                                new Align
                                {
                                    new Group
                                    {
                                        new Ellipse()
                                            .FillColor(CurrentTheme.Surface),
                                        new Text("۰۰۰")
                                            .VerticalAlignment(VerticalAlignment.Center)
                                            .HorizontalAlignment(HorizontalAlignment.Center)
                                            .FontSize(20)
                                            .FontWeight(FontWeights.UltraBold)
                                            .FontColor(CurrentTheme.OnSurface)
                                    }
                                }
                                .HEnd()
                                .Width(32)
                                .Height(32)
                            }
                        }
                        .VCenter()
                        .Height(32)

                    }
                    .Margin(0,24,0,0),

                    new Column("20 *")
                    {
                        new Text("About event")
                            .FontSize(20)
                            .FontWeight(FontWeights.Bold)
                            .FontColor(CurrentTheme.OnBackground),

                        new Text("Nunc vitae pharetra bibendum ultrices. Ornare amet aliquam aenean viverra ut tellus.\r\n\r\nCras aliquam nisi, risus enim amet. Sed pellentesque mauris, eget urna id ut sed vitae. Erat facilisi purus ut id in pulvinar sed sit. Vestibulum convallis consectetur quis eget netus magna ultrices adipiscing. Ornare sit semper cras lorem nec. Metus consectetur nunc aliquam in non.")
                            .FontSize(12)
                            .FontColor(CurrentTheme.OnSurface)
                    }
                    .Margin(0,30,0,0)
                }
                .Margin(16)
            }
            .CornerRadius(24, 24, 0, 0)
            .BackgroundColor(CurrentTheme.Background)
        }
        .Opacity(State.ImageOpacity)
        .Margin(0, State.MainPanelX, 0, 0)
        .WithAnimation(duration: 200)
        .BackgroundColor(Colors.Transparent);

    VisualNode RenderBottomPanel()
        => new CanvasView()
        {
            new Box()
            {
                new Row("56, *")
                {
                    new Box
                    {
                        new Align
                        {
                            new Picture($"PayItGlobal.App.Resources.Images.fav_small.png")
                        }
                        .Height(24)
                        .Width(24)
                        .HCenter()
                        .VCenter()
                    }
                    .CornerRadius(12)
                    .BackgroundColor(CurrentTheme.Surface),

                    new Box
                    {
                        new Text("Join to event")
                            .FontColor(CurrentTheme.OnPrimary)
                            .FontSize(16)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .VerticalAlignment(VerticalAlignment.Center)
                    }
                    .Margin(16,0,0,0)
                    .CornerRadius(12)
                    .BackgroundColor(Colors.Transparent)
                }
                .Margin(16,24)
            }
            .CornerRadius(24, 24, 0, 0)
            .BackgroundColor(CurrentTheme.Background)
        }
        .BackgroundColor(Colors.Transparent)
        .HeightRequest(104)
        .GridRow(1);

    private async void OnBack()
    {
        if (Navigation != null && Navigation.NavigationStack.Count > 0)
        {
            await Navigation.PopAsync();
        }
    }
}
