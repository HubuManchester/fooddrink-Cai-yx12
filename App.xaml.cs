namespace RecipeRandomizer;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // 临时：先清空登录状态，确保看到登录页
        Preferences.Set("IsLoggedIn", false);

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
}