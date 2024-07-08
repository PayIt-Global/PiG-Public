//using MauiReactor;
//using PayItGlobal.App.Pages;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PayItGlobal.App.Services
//{
//    public class NavigationService : INavigationService
//    {
//        private readonly Dictionary<string, Func<VisualNode>> _pagesByKey = new Dictionary<string, Func<VisualNode>>();

//        public NavigationService()
//        {
//            // Register pages
//            _pagesByKey.Add("MainPage", () => new MainPage());
//            _pagesByKey.Add("LoginPage", () => new LoginPage());
//            // Add more pages as needed
//        }

//        public void NavigateTo(string pageKey)
//        {
//            if (_pagesByKey.TryGetValue(pageKey, out var pageConstructor))
//            {
//                var page = pageConstructor();
//                // Assuming you have a way to access the current NavigationPage
//                var navigationPage = App.Current.MainPage as NavigationPage;
//                navigationPage?.PushAsync(page.ToPage());
//            }
//        }

//        public void GoBack()
//        {
//            var navigationPage = App.Current.MainPage as NavigationPage;
//            navigationPage?.PopAsync();
//        }
//    }

//}
