namespace RecipeRandomizer.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        LoginButton.Clicked += OnLoginClicked;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(username))
        {
            ErrorLabel.Text = "Please enter username";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (!string.IsNullOrEmpty(username))
        {
            Preferences.Set("IsLoggedIn", true);
            Preferences.Set("Username", username);

            Application.Current!.MainPage = new AppShell();
        }
        else
        {
            ErrorLabel.Text = "Invalid username or password";
            ErrorLabel.IsVisible = true;
        }
    }
}