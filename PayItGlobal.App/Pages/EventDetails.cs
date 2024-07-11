using MauiReactor;
using MauiReactor.Canvas;

namespace PayItGlobal.App.Pages;

class EventDetails : Component
{
    public override VisualNode Render()
    {
        return new Text("This a Text element!")
                                .HorizontalAlignment(HorizontalAlignment.Center)
                                .VerticalAlignment(VerticalAlignment.Center)
                                .FontColor(Colors.Black)
                                .FontSize(14);
    }
}
