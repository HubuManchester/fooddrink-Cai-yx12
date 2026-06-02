namespace RecipeRandomizer;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

        if (isLoggedIn)
        {
            return new Window(new AppShell());
        }
        else
        {
            return new Window(new Views.LoginPage());
        }
    }

    public static void Logout()
    {
        Preferences.Set("IsLoggedIn", false);
        Current!.MainPage = new Views.LoginPage();
    }
}