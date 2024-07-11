using MauiReactor;

namespace PayItGlobal.App.Pages
{
    internal class NotFoundPage : Component
    {
        public override VisualNode Render()
            => ContentPage(
                ScrollView(
                    VStack(
                        Label("NotFound Page")
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
