using MauiReactor;

namespace PayItGlobal.App.Pages
{
    internal class CalendarPage : Component
    {
        public override VisualNode Render()
            => ContentPage(
                ScrollView(
                    VStack(
                        Label("Calendar Page")
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
