using MauiReactor;
using MauiReactor.Canvas;
using PayItGlobal.App.Pages;
using PayItGlobal.App.Resources.Styles;
using System;
using System.Linq;

namespace PayItGlobal.App
{
    internal class AppShell : Component
    {
        private PageEnum _currentPage = PageEnum.HomePage; // Initial page

        public override VisualNode Render()
        {
            return new VStack
            {
                // Your pages or content here based on _currentPage
                RenderPageContent(),

                // Custom tab bar at the bottom
                RenderCustomTabBar()
            };
        }

        private VisualNode RenderPageContent()
        {
            // Switch between pages based on _currentPage
            switch (_currentPage)
            {
                case PageEnum.HomePage:
                    return new HomePage();
                case PageEnum.EventsPage:
                    return new EventsPage();
                default:
                    return new NotFoundPage();
            }
        }

        private VisualNode RenderCustomTabBar()
        {
            ImageButton createButton(PageEnum page, int column) =>
                new ImageButton()
                    .Aspect(Aspect.Center)
                    .Source(() => _currentPage != page ? $"{page.ToString().ToLowerInvariant()}.png" : $"{page.ToString().ToLowerInvariant()}_on.png")
                    .GridColumn(column)
                    .OnClicked(() => UpdateCurrentPage(page))
                    .Margin(_currentPage == page ? new Thickness(0, 0, 0, 10) : new Thickness())
                    .WithAnimation(Easing.BounceOut, 300);

            return new Grid("*", "*")
            {
                // Your Grid implementation
            }
            .VEnd()
            .HeightRequest(92);
        }

        private void UpdateCurrentPage(PageEnum page)
        {
            if (_currentPage != page)
            {
                _currentPage = page;
                Invalidate(); // Trigger a re-render of the component
            }
        }
    }
}
