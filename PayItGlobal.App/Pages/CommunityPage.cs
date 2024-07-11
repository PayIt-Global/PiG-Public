using MauiReactor;

namespace PayItGlobal.App.Pages
{
    internal class CommunityPage : Component
    {
        public override VisualNode Render()
            => ContentPage(
                ScrollView(
                    VStack(
                        Label("Community Page")
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
