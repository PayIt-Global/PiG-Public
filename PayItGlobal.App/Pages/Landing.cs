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

class LandingPageState
{

}

class Landing : Component
{
    public override VisualNode Render()
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
        };

        //return new Grid("268, *, 92", "*")
        //{
        //    RenderTopPanel()
        //}
        //.Margin(0, 0, 0, 88);
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
                    new Grid("*,*", "*")
                    {
                        new Label("label1")
                            .Text("Hello,")
                            .FontSize(24)
                            .TextColor(ThemeBrushes.White)
                            .Margin(0,0,0,8)
                            .GridRow(0),

                        new Label("label2")
                            .Text("John Doe")
                            .FontSize(24)
                            .TextColor(ThemeBrushes.White)
                            .GridRow(1)
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
