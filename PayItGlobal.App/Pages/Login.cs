using MauiReactor;
using PayItGlobal.App.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages;

class LoginState
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsLoggingIn { get; set; } = false;
}

partial class LoginPage : Component<LoginState>
{
    [Inject]
    private IApiSettingsService _apiSettingsService;

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
            await MauiControls.Shell.Current.GoToAsync("main");
        }
        else
        {
            // Handle login failure
        }

        SetState(_ => _.IsLoggingIn = false);
    }

    private async Task<bool> AuthenticateUser(string username, string password)
    {
        var apiSettings = _apiSettingsService.GetSettings();

        // Construct the full URL for the login endpoint
        string loginUrl = $"{apiSettings.BaseUrl}{apiSettings.LoginEndpoint}";

        using (var httpClient = new HttpClient())
        {
            // Prepare the request content
            var requestData = new { Username = username, Password = password };
            var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request to the login endpoint
                var response = await httpClient.PostAsync(loginUrl, requestContent);

                // Ensure we received a successful response
                if (response.IsSuccessStatusCode)
                {
                    // Optionally, read the response content if needed (e.g., to retrieve a token)
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize the response content if necessary
                    // Assuming the API returns an object with Token and RefreshToken properties
                    var responseData = JsonSerializer.Deserialize<dynamic>(responseContent);

                    // Store or use the token as needed
                    // For example, storing the token in a secure place for future API calls
                    // var token = responseData.Token;

                    // Authentication succeeded
                    return true;
                }
                else
                {
                    // Handle non-successful response, such as unauthorized or server error
                    return false;
                }
            }
            catch (HttpRequestException e)
            {
                // Handle request exception
                Console.WriteLine($"Error making HTTP request: {e.Message}");
                return false;
            }
        }
    }


}
