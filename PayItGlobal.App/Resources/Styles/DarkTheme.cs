using Microsoft.Maui.Controls;
using PayItGlobalApi.App.Themes;

namespace PayItGlobalApi.App.Resources.Styles
{
    public class DarkTheme : IThemeColors
    {
        public Color Primary => Color.FromRgb(136, 209, 235);
        public Color SurfaceTint => Color.FromRgb(136, 209, 235);
        public Color OnPrimary => Color.FromRgb(0, 53, 67);
        public Color PrimaryContainer => Color.FromRgb(0, 78, 96);
        public Color OnPrimaryContainer => Color.FromRgb(182, 234, 255);
        public Color Secondary => Color.FromRgb(179, 202, 212);
        public Color OnSecondary => Color.FromRgb(30, 51, 59);
        public Color SecondaryContainer => Color.FromRgb(52, 74, 82);
        public Color OnSecondaryContainer => Color.FromRgb(207, 230, 241);
        public Color Tertiary => Color.FromRgb(195, 195, 235);
        public Color OnTertiary => Color.FromRgb(44, 46, 77);
        public Color TertiaryContainer => Color.FromRgb(66, 68, 101);
        public Color OnTertiaryContainer => Color.FromRgb(224, 224, 255);
        public Color Error => Color.FromRgb(255, 180, 171);
        public Color OnError => Color.FromRgb(105, 0, 5);
        public Color ErrorContainer => Color.FromRgb(147, 0, 10);
        public Color OnErrorContainer => Color.FromRgb(255, 218, 214);
        public Color Background => Color.FromRgb(15, 20, 22);
        public Color OnBackground => Color.FromRgb(222, 227, 230);
        public Color Surface => Color.FromRgb(15, 20, 22);
        public Color OnSurface => Color.FromRgb(222, 227, 230);
        public Color SurfaceVariant => Color.FromRgb(64, 72, 76);
        public Color OnSurfaceVariant => Color.FromRgb(191, 200, 204);
        public Color Outline => Color.FromRgb(138, 146, 150);
        public Color OutlineVariant => Color.FromRgb(64, 72, 76);
        public Color Shadow => Color.FromRgb(0, 0, 0);
        public Color Scrim => Color.FromRgb(0, 0, 0);
        public Color InverseSurface => Color.FromRgb(222, 227, 230);
        public Color InverseOnSurface => Color.FromRgb(44, 49, 52);
        public Color InversePrimary => Color.FromRgb(6, 103, 126);
        // Add more colors as needed...
    }
}
