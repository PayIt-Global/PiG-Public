using System;
using MauiReactor;
using MauiReactor.Canvas;
using PayItGlobal.App.Themes;

namespace PayItGlobal.App.Pages;

class CalendarPageState
{

}

class Calendar : Component
{
    private IThemeColors CurrentTheme => ThemeManager.CurrentTheme; 

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
        return new Grid
        {
            new CanvasView
            {
                new Picture("PayItGlobal.App.Resources.Images.top.png")
                    .Aspect(Aspect.Fill),

                new Align
                {
                    new Group
                    {
                        new ClipRectangle
                        {
                            new Picture("PayItGlobal.App.Resources.Images.photo1.png")
                        }
                        .CornerRadius(16),

                        new Align()
                        {
                            new Ellipse()
                                .StrokeColor(CurrentTheme.Primary) // Best guess: Primary for the stroke
                                .FillColor(CurrentTheme.Secondary) // Best guess: Secondary for the fill
                                .StrokeSize(5)
                        }
                        .VEnd()
                        .HStart()
                        .Width(13)
                        .Height(13)
                    }
                }
                .Height(48)
                .Width(48)
                .HCenter()
                .VStart()
                .Margin(0,72,0,0),

                new Align
                {
                    new Column("23, *")
                    {
                        new Text("Calendar")
                            .FontColor(CurrentTheme.OnBackground) // Best guess: OnBackground for text color
                            .FontSize(18)
                            .HorizontalAlignment(HorizontalAlignment.Center),
                    }
                }
                .Height(61)
                .VEnd()
                .Margin(0,0,0,56)
            }
            .VStart()
            .HeightRequest(253)
            .Background(Colors.Transparent)

        }
        .GridRow(0);
    }
}
