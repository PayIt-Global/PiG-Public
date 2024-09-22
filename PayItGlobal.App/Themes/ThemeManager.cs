using PayItGlobalApi.App.Resources.Styles;

namespace PayItGlobalApi.App.Themes
{
    public static class ThemeManager
    {
        // This property holds the current theme. You might want to load the initial value from user preferences or default settings.
        public static IThemeColors CurrentTheme { get; set; } = new LightTheme(); // Assume DefaultTheme implements IThemeColors

        // Add methods here to change the theme, if necessary.
    }
}
