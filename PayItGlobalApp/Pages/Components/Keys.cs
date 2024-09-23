using MauiReactor;
using MauiReactor.Shapes;
using PayItGlobalApp.Resources;
using System;
using Theme = PayItGlobalApp.Resources.Theme;

namespace PayItGlobalApp.Pages.Components;

partial class Keys : Component
{
    public override VisualNode Render()
    {
        return Grid("*", "*",
            Label("Keys Page Content")
                .FontSize(24)
                .TextColor(Colors.Black)
                .HCenter()
                .VCenter()
        )
        .BackgroundColor(Theme.Background);
    }
}
