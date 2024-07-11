using MauiReactor;

namespace PayItGlobal.App.Pages
{
    internal class EventsPage : Component
    {
        public override VisualNode Render()
            => ContentPage(
                ScrollView(
                    VStack(
                        Label("Events Page")
                            .FontSize(32)
                            .HCenter()
                    )
                    .VCenter()
                    .Spacing(25)
                    .Padding(30, 0)
                )
            );
    }
}
