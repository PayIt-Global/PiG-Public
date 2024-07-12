﻿using MauiReactor;
using MauiReactor.Canvas;
using MauiReactor.Parameters;
using Microsoft.Extensions.DependencyInjection;
using PayItGlobal.App.Resources.Styles;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.DTOs.Shared;
using System;
using System.Linq;

namespace PayItGlobal.App.Pages;

enum PageEnum
{
    Home,

    Events,

    Community,

    Assets,

    Calendar
}


class MainPageState
{
    public PageEnum CurrentPage { get; set; } = PageEnum.Home;
    public bool IsAuthenticated { get; set; }
    public bool Loading { get; set; }
    public bool LoadedDirectly { get; set; } = true; // Default to true
}

class MainPage : Component<MainPageState>
{
    private readonly IParameter<MainPageState> _mainState;
    public MainPage()
    {
        _mainState = CreateParameter<MainPageState>();

    }
    protected override async void OnMounted()
    {
        var authService = Services.GetRequiredService<IClientAuthenticationService>();

        // Start loading
        SetState(s => s.Loading = true);

        try
        {
            // Perform the async operation
            bool isAuthenticated = await authService.IsLoggedInAsync();
            // Update the state once the async operation is complete
            SetState(s =>
            {
                s.IsAuthenticated = isAuthenticated;
                s.Loading = false;
            });
        }
        catch (Exception ex)
        {
            // Handle any errors that occurred during the async operation
            // For example, you might want to log the error and update the state to reflect that an error occurred
            Console.WriteLine($"An error occurred: {ex.Message}");
            SetState(s =>
            {
                s.Loading = false;
                // Optionally, add an error state to MainPageState and set it here
            });
        }

        base.OnMounted();
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

                    RenderTabBar()
                }
            }
            .Set(MauiControls.NavigationPage.HasNavigationBarProperty, false)
            .BackgroundColor(ThemeBrushes.Background)
        };
    }

    VisualNode RenderPage()
    {
        if (!State.IsAuthenticated)
        {
            return new Landing(); 
        }

        return State.CurrentPage switch
        {
            PageEnum.Home => new Home(),
            PageEnum.Events => new Events(),
            PageEnum.Community => new Community(),
            PageEnum.Assets => new Assets(),
            PageEnum.Calendar => new Calendar(),
            _ => throw new NotImplementedException(),
        };
    }

    VisualNode RenderTabBar()
    {
        ImageButton createButton(PageEnum page, int column) =>
            new ImageButton()
                .Aspect(Aspect.Center)
                .Source(() => State.CurrentPage != page ? $"{page.ToString().ToLowerInvariant()}.png" : $"{page.ToString().ToLowerInvariant()}_on.png")
                .GridColumn(column)
                .OnClicked(() => SetState(s => s.CurrentPage = page))
                .Margin(State.CurrentPage == page ? new Thickness(0, 0, 0, 10) : new Thickness())
                .WithAnimation(Easing.BounceOut, 300)
                ;

        return new Grid("*", "*")
        {
            new CanvasView()
            {
                new DropShadow
                {
                    new Box()
                        .Margin(0,20,0,0)
                        .CornerRadius(24,24,0,0)
                        .BackgroundColor (ThemeBrushes.White)
                }
                .Color(ThemeBrushes.DarkShadow)
                .Size(0, -8)
                .Blur(32),

                new Row("* * * * *")
                {
                    Enum.GetValues(typeof(PageEnum))
                        .Cast<PageEnum>()
                        .Select(page =>
                            new Align()
                            {
                                new Ellipse()
                                    .FillColor(ThemeBrushes.Purple10)
                            }
                            .IsVisible(State.CurrentPage == page)
                            .Width(4)
                            .Height(4)
                            .VEnd()
                            .HCenter()
                            .Margin(16)
                        )
                        .ToArray()
                }

            }
            .BackgroundColor(Colors.Transparent)
            .GridRow(1),

            new Grid("*", "* * * * *")
            {
                createButton(PageEnum.Home, 0),
                createButton(PageEnum.Events, 1),
                createButton(PageEnum.Community, 2),
                createButton(PageEnum.Assets, 3),
                createButton(PageEnum.Calendar, 4)
            }
            .Padding(0,20,0,0)
        }
        .VEnd()
        .HeightRequest(92);
    }
}
