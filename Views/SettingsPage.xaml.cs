using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        LargeTextSwitch.IsToggled = AccessibilityService.LargeTextEnabled;
        DarkThemeSwitch.IsToggled = AccessibilityService.IsDarkTheme;

        LargeTextSwitch.Toggled += OnLargeTextToggled;
        DarkThemeSwitch.Toggled += OnDarkThemeToggled;

        UpdatePreview();
        ApplyTheme();
        ApplyFontScaling();
    }

    private void OnLargeTextToggled(object? sender, ToggledEventArgs e)
    {
        AccessibilityService.ToggleLargeText();
        UpdatePreview();
        ApplyFontScaling();

        // 刷新主页面的字体
        RefreshMainPageFonts();

        DisplayAlert("Accessibility",
            e.Value ? "Large text mode enabled" : "Large text mode disabled", "OK");
    }

    private void OnDarkThemeToggled(object? sender, ToggledEventArgs e)
    {
        AccessibilityService.ToggleTheme();
        ApplyTheme();

        // 刷新主页面的主题
        RefreshMainPageTheme();

        DisplayAlert("Accessibility",
            e.Value ? "Dark mode enabled" : "Light mode enabled", "OK");
    }

    private void RefreshMainPageFonts()
    {
        var mainPage = Application.Current?.MainPage as NavigationPage;
        if (mainPage?.CurrentPage is MainPage mp)
        {
            mp.RefreshFonts();
        }
    }

    private void RefreshMainPageTheme()
    {
        var mainPage = Application.Current?.MainPage as NavigationPage;
        if (mainPage?.CurrentPage is MainPage mp)
        {
            mp.ApplyTheme();
        }
    }

    private void ApplyFontScaling()
    {
        TitleLabel.FontSize = AccessibilityService.ScaleFontSize(20);
        LargeTextLabel.FontSize = AccessibilityService.ScaleFontSize(16);
        DarkModeLabel.FontSize = AccessibilityService.ScaleFontSize(16);
        PreviewLabel.FontSize = AccessibilityService.ScaleFontSize(14);
    }

    private void UpdatePreview()
    {
        PreviewLabel.FontSize = AccessibilityService.ScaleFontSize(14);
        PreviewLabel.Text = AccessibilityService.LargeTextEnabled
            ? "🔤 Large text mode ON"
            : "Standard text size";
    }

    private void ApplyTheme()
    {
        if (AccessibilityService.IsDarkTheme)
        {
            this.BackgroundColor = Color.FromArgb("#1E1E1E");
            TitleLabel.TextColor = Colors.White;
            LargeTextLabel.TextColor = Colors.White;
            DarkModeLabel.TextColor = Colors.White;
            PreviewLabel.TextColor = Colors.LightGray;
            SettingsFrame.BackgroundColor = Color.FromArgb("#2D2D2D");
        }
        else
        {
            this.BackgroundColor = Color.FromArgb("#F5F5F5");
            TitleLabel.TextColor = Colors.Black;
            LargeTextLabel.TextColor = Colors.Black;
            DarkModeLabel.TextColor = Colors.Black;
            PreviewLabel.TextColor = Colors.Gray;
            SettingsFrame.BackgroundColor = Colors.White;
        }
    }
}