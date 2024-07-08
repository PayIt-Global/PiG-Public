using MauiReactor;

namespace PayItGlobal.App.Pages;

class LoginPageState
{
    public string Username { get; set; }
    public string Password { get; set; }
}

class LoginPage : Component<LoginPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage("LoginPage")
        {
            new VStack // Use a vertical stack layout to organize elements
            {
                new Entry()
                    .Placeholder("Username")
                    .OnTextChanged((s,e)=> SetState(_ => _.Username = e.NewTextValue)),
                new Entry()
                    .Placeholder("Password")
                    .OnTextChanged((s,e)=> SetState(_ => _.Password = e.NewTextValue))
                    .IsPassword(true),
                new Button("Login")
                    .IsEnabled(!string.IsNullOrWhiteSpace(State.Username) && !string.IsNullOrWhiteSpace(State.Password))
                    .OnClicked(OnLogin),
                new Button("Go back")
                    .HCenter()
                    .VCenter()
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(".."))
            }
            .Spacing(20) // Adjust spacing between elements
            .Padding(30) // Adjust padding around the stack
            .VCenter()
            .HCenter()
        };
    }

    private void OnLogin()
    {
        // Use State.Username and State.Password to login...
        // Placeholder for login logic
    }
}
