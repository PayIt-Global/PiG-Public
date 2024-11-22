using MauiReactor;
using MauiReactor.Shapes;
using CryptAplyApp.Resources;
using System;
using Theme = CryptAplyApp.Resources.Theme;

namespace CryptAplyApp.Pages.Components;

partial class Help : Component
{
    public override VisualNode Render()
    {
        return Grid("*", "*",
            Label("Help Page Content")
                .FontSize(24)
                .TextColor(Colors.Black)
                .HCenter()
                .VCenter()
        )
        .BackgroundColor(Theme.Background);
    }
}
