using Microsoft.Maui.Controls;
using PayItGlobalApi.App.Themes;

namespace PayItGlobalApi.App.Resources.Styles
{
    public class LightTheme : IThemeColors
    {
        public Color Primary => Color.FromRgb(6, 103, 126);
        public Color SurfaceTint => Color.FromRgb(6, 103, 126);
        public Color OnPrimary => Color.FromRgb(255, 255, 255);
        public Color PrimaryContainer => Color.FromRgb(182, 234, 255);
        public Color OnPrimaryContainer => Color.FromRgb(0, 31, 40);
        public Color Secondary => Color.FromRgb(76, 98, 106);
        public Color OnSecondary => Color.FromRgb(255, 255, 255);
        public Color SecondaryContainer => Color.FromRgb(207, 230, 241);
        public Color OnSecondaryContainer => Color.FromRgb(7, 30, 38);
        public Color Tertiary => Color.FromRgb(90, 92, 126);
        public Color OnTertiary => Color.FromRgb(255, 255, 255);
        public Color TertiaryContainer => Color.FromRgb(224, 224, 255);
        public Color OnTertiaryContainer => Color.FromRgb(23, 25, 55);
        public Color Error => Color.FromRgb(186, 26, 26);
        public Color OnError => Color.FromRgb(255, 255, 255);
        public Color ErrorContainer => Color.FromRgb(255, 218, 214);
        public Color OnErrorContainer => Color.FromRgb(65, 0, 2);
        public Color Background => Color.FromRgb(245, 250, 253);
        public Color OnBackground => Color.FromRgb(23, 28, 31);
        public Color Surface => Color.FromRgb(245, 250, 253);
        public Color OnSurface => Color.FromRgb(23, 28, 31);
        public Color SurfaceVariant => Color.FromRgb(219, 228, 232);
        public Color OnSurfaceVariant => Color.FromRgb(64, 72, 76);
        public Color Outline => Color.FromRgb(112, 120, 124);
        public Color OutlineVariant => Color.FromRgb(191, 200, 204);
        public Color Shadow => Color.FromRgb(0, 0, 0);
        public Color Scrim => Color.FromRgb(0, 0, 0);
        public Color InverseSurface => Color.FromRgb(44, 49, 52);
        public Color InverseOnSurface => Color.FromRgb(237, 241, 244);
        public Color InversePrimary => Color.FromRgb(136, 209, 235);
        // Add more colors as needed...
    }
}
