//using PayItGlobal.App.Pages;
//using Microsoft.Maui.Controls;
//using System;
//using Microsoft.Extensions.DependencyInjection;

//namespace PayItGlobal.App.Services
//{
//    public class PageFactory : IPageFactory
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public PageFactory(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        public Page CreateMainPage()
//        {
//            // Resolve INavigationService from the service provider
//            var navigationService = _serviceProvider.GetService<INavigationService>();
//            if (navigationService == null)
//            {
//                throw new InvalidOperationException("Navigation service not found.");
//            }

//            // Pass the resolved INavigationService to the MainPage constructor
//            return new MainPage(navigationService);
//        }
//    }
//}
