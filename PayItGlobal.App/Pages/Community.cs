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

namespace PayItGlobal.App.Pages;

class CommunityPageState
{

}

class Community : Component
{
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
                new Picture("Contentics.Resources.Images.top.png")
                    .Aspect(Aspect.Fill),

                new Align
                {
                    new Group
                    {
                        new ClipRectangle
                        {
                            new Picture("Contentics.Resources.Images.photo1.png")
                        }
                        .CornerRadius(16),

                        new Align()
                        {
                            new Ellipse()
                                .StrokeColor(ThemeBrushes.Purple10)
                                .FillColor(ThemeBrushes.Green)
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
                        new Text("Community")
                            .FontColor(ThemeBrushes.Purple30)
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
