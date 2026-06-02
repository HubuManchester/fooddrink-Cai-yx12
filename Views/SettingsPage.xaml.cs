using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        LargeTextSwitch.IsToggled = AccessibilityService.LargeTextEnabled;
        DarkModeSwitch.IsToggled = AccessibilityService.IsDarkTheme;

        LargeTextSwitch.Toggled += OnLargeTextToggled;
        DarkModeSwitch.Toggled += OnDarkModeToggled;

        ApplyTheme();
    }

    private async void OnLargeTextToggled(object? sender, ToggledEventArgs e)
    {
        AccessibilityService.ToggleLargeText();
        RefreshMainPage();
        await DisplayAlert("Accessibility",
            e.Value ? "Large text mode enabled" : "Large text mode disabled", "OK");
    }

    private async void OnDarkModeToggled(object? sender, ToggledEventArgs e)
    {
        AccessibilityService.ToggleTheme();
        ApplyTheme();
        RefreshMainPage();
        await DisplayAlert("Accessibility",
            e.Value ? "Dark mode enabled" : "Light mode disabled", "OK");
    }

    private void RefreshMainPage()
    {
        var mainPage = Application.Current?.MainPage as NavigationPage;
        if (mainPage?.CurrentPage is MainPage mp)
        {
            mp.RefreshFonts();
            mp.ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        if (AccessibilityService.IsDarkTheme)
        {
            this.BackgroundColor = Color.FromArgb("#1E1E1E");
        }
        else
        {
            this.BackgroundColor = Color.FromArgb("#F5F5F5");
        }
    }
}