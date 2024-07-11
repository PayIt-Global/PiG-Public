using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Parameters;
using PayItGlobal.App.Resources.Styles;
using PayItGlobal.DTOs.Shared;
using System;
using System.Linq;

namespace PayItGlobal.App.Pages
{
    enum PageEnum
    {
        HomePage,

        EventsPage,

        CommunityPage,

        AssetsPage,

        CalendarPage
    }

    class MainPageState
    {
        public PageEnum CurrentPage { get; set; }

        public bool Loading { get; set; }
    }
    class MainState
    {
        public UserViewModel? CurrentUser { get; set; }
    }

    internal class MainPage : Component<MainPageState>
    {
        private readonly IParameter<MainState> _mainState;
        public MainPage()
        {
            _mainState = CreateParameter<MainState>();
        }
        public override VisualNode Render()
        {
            return new NavigationPage
            {
                new ContentPage
                {
                    new Grid("*", "*")
                    {
                        RenderPage(),

                        // RenderTabBar() call removed
                    }
                }
                .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false)
                .BackgroundColor(ThemeBrushes.Background)
            };
        }
        VisualNode RenderPage() => State.CurrentPage switch
        {
            PageEnum.HomePage => new HomePage(),
            _ => throw new NotImplementedException(),
        };
        // RenderTabBar method removed
    }
}
