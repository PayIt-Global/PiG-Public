using MauiReactor;
using Microsoft.Maui.Graphics;
using PayItGlobal.App.Resources.Styles;
using PayItGlobal.App.Themes;

class AppTheme : Theme
{
    private static IThemeColors CurrentThemeColors = new LightTheme(); // Default to light theme

    public static void ToggleCurrentAppTheme()
    {
        // Toggle between Light and Dark themes
        CurrentThemeColors = (CurrentThemeColors is LightTheme) ? new DarkTheme() : new LightTheme();

        // Apply the theme changes
        ApplyTheme();
    }

    public static void ApplyTheme()
    {
        // Since there's no base Apply to override, we directly call OnApply
        new AppTheme().OnApply();
    }

    protected override void OnApply()
    {
        // Apply the theme colors to various UI elements
        ContentPageStyles.Default = _ => _
            .BackgroundColor(CurrentThemeColors.Background);

        LabelStyles.Default = _ => _
            .TextColor(CurrentThemeColors.OnBackground);

        // Apply other styles based on the current theme colors
    }
}
