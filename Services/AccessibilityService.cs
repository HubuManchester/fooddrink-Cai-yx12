namespace RecipeRandomizer.Services;

public static class AccessibilityService
{
    public static bool LargeTextEnabled { get; private set; } = false;
    public static bool IsDarkTheme { get; private set; } = false;

    public static void ToggleLargeText()
    {
        LargeTextEnabled = !LargeTextEnabled;
        System.Diagnostics.Debug.WriteLine($"LargeTextEnabled: {LargeTextEnabled}");
    }

    public static void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        System.Diagnostics.Debug.WriteLine($"IsDarkTheme: {IsDarkTheme}");
    }

    public static double ScaleFontSize(double originalSize)
    {
        var result = LargeTextEnabled ? originalSize * 1.25 : originalSize;
        System.Diagnostics.Debug.WriteLine($"ScaleFontSize: {originalSize} -> {result}");
        return result;
    }
}