using Microsoft.Maui.Controls;

namespace PayItGlobalApi.App.Themes
{
    public interface IThemeColors
    {
        Color Primary { get; }
        Color SurfaceTint { get; }
        Color OnPrimary { get; }
        Color PrimaryContainer { get; }
        Color OnPrimaryContainer { get; }
        Color Secondary { get; }
        Color OnSecondary { get; }
        Color SecondaryContainer { get; }
        Color OnSecondaryContainer { get; }
        Color Tertiary { get; }
        Color OnTertiary { get; }
        Color TertiaryContainer { get; }
        Color OnTertiaryContainer { get; }
        Color Error { get; }
        Color OnError { get; }
        Color ErrorContainer { get; }
        Color OnErrorContainer { get; }
        Color Background { get; }
        Color OnBackground { get; }
        Color Surface { get; }
        Color OnSurface { get; }
        Color SurfaceVariant { get; }
        Color OnSurfaceVariant { get; }
        Color Outline { get; }
        Color OutlineVariant { get; }
        Color Shadow { get; }
        Color Scrim { get; }
        Color InverseSurface { get; }
        Color InverseOnSurface { get; }
        Color InversePrimary { get; }
        // Add more color properties as needed...
    }
}