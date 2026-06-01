namespace RecipeRandomizer.Services;

public static class AccessibilityService
{
    public static bool LargeTextEnabled { get; private set; } = false;
    public static bool IsDarkTheme { get; private set; } = false;

    public static void ToggleLargeText()
    {
        LargeTextEnabled = !LargeTextEnabled;
    }

    public static void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
    }

    public static double ScaleFontSize(double originalSize)
    {
        return LargeTextEnabled ? originalSize * 1.3 : originalSize;
    }
}