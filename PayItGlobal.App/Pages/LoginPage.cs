using MauiReactor;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages;

class LoginPageState
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsLoggingIn { get; set; } = false;
}

class LoginPage : Component<LoginPageState>
{
    protected override void OnMounted()
    {
        base.OnMounted();
        Routing.RegisterRoute<MainPage>("main-page");
    }

    public override VisualNode Render()
    {
        return new ContentPage("LoginPage")
        {
            new VStack
            {
                new Entry()
                    .Placeholder("Username")
                    .OnTextChanged((s,e)=> SetState(_ => _.Username = e.NewTextValue)),
                new Entry()
                    .Placeholder("Password")
                    .OnTextChanged((s,e)=> SetState(_ => _.Password = e.NewTextValue))
                    .IsPassword(true),
                new Button("Login")
                    .IsEnabled(CanLogin())
                    .OnClicked(async () => await AttemptLogin()),
                new Button("Go back")
                    .HCenter()
                    .VCenter()
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(".."))
            }
            .Spacing(20)
            .Padding(30)
            .VCenter()
            .HCenter()
        };
    }

    private bool CanLogin() => !string.IsNullOrWhiteSpace(State.Username) && !string.IsNullOrWhiteSpace(State.Password) && !State.IsLoggingIn;

    private async Task AttemptLogin()
    {
        SetState(_ => _.IsLoggingIn = true);

        // Mock authentication logic
        bool isAuthenticated = await AuthenticateUser(State.Username, State.Password);

        if (isAuthenticated)
        {
            // Navigate to the MainPage using the registered route
            await MauiControls.Shell.Current.GoToAsync("main-page");
        }
        else
        {
            // Handle login failure
        }

        SetState(_ => _.IsLoggingIn = false);
    }

    private Task<bool> AuthenticateUser(string username, string password)
    {
        // This is a placeholder for your authentication logic
        // For demonstration, let's assume any non-empty credentials are valid
        bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
        return Task.FromResult(isValid);
    }
}
