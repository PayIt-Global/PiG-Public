//using MauiReactor;
//using System.Reactive;

//namespace PayItGlobal.App.Pages;

//class LoginPageState
//{
//    public string Username { get; set; } = string.Empty;
//    public string Password { get; set; } = string.Empty;
//}

//class LoginPage : Component<LoginPageState>
//{
//    public event EventHandler<Unit>? OnLoginSuccess;

//    private void HandleLogin()
//    {
//        // Here, you would call your AuthenticationService to perform the login
//        // For demonstration, let's assume the login is always successful
//        OnLoginSuccess?.Invoke(this, Unit.Default);

//        // Navigate to MainPage or show success message
//    }

//    public override VisualNode Render()
//        => ContentPage(
//                ScrollView(
//                    VStack(
//                        Label("Login")
//                            .FontSize(32)
//                            .HCenter(),

//                        Entry()
//                            .Text(State.Username)
//                            .Placeholder("Username")
//                            .OnTextChanged(text => SetState(s => s.Username = text)),

//                        Entry()
//                            .Text(State.Password)
//                            .Placeholder("Password")
//                            .IsPassword(true)
//                            .OnTextChanged(text => SetState(s => s.Password = text)),

//                        Button("Login")
//                            .OnClicked(HandleLogin)
//                            .HCenter()
//                    )
//                    .VCenter()
//                    .Spacing(25)
//                    .Padding(30, 0)
//                )
//            );
//}
