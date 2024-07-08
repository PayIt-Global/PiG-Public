
using MauiReactor;

namespace PayItGlobal.App.Pages;

class MainPageState
{
    public int Counter { get; set; }
}

class MainPage : Component
{
    protected override void OnMounted()
    {
        Routing.RegisterRoute<LoginPage>("LoginPage");
        base.OnMounted();
    }

    public override VisualNode Render()
        => new Shell()
        {
            new FlyoutItem("Page1")
            {
                new ShellContent()
                    .RenderContent(() => new ContentPage("Page1")
                    {
                        new Button("Goto to LoginPage")
                            .HCenter()
                            .VCenter()
                        .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync("LoginPage"))
                    })
            }
        }
        .ItemTemplate(RenderItemTemplate);

    static VisualNode RenderItemTemplate(MauiControls.BaseShellItem item)
        => new Grid("68", "*")
        {
            new Label(item.Title)
                .VCenter()
                .Margin(10,0)
        };
}


