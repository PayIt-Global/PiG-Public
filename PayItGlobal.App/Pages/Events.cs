using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Compatibility;
using Microsoft.Maui.Devices;

namespace PayItGlobalApi.App.Pages;
using PayItGlobalApi.App.Themes;
class EventsPageState
{

}

class Events : Component
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
                                .StrokeColor(CurrentTheme.Primary)
                                .FillColor(CurrentTheme.Secondary)
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
                        new Text("Events")
                            .FontColor(CurrentTheme.OnBackground)
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
