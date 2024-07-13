using Microsoft.Maui.Controls;
using PayItGlobal.App.Themes;

namespace PayItGlobal.App.Resources.Styles
{
    public class LightMediumContrastTheme : IThemeColors
    {
        public Color Primary => Color.FromRgb(0, 73, 91);
        public Color SurfaceTint => Color.FromRgb(6, 103, 126);
        public Color OnPrimary => Color.FromRgb(255, 255, 255);
        public Color PrimaryContainer => Color.FromRgb(46, 126, 150);
        public Color OnPrimaryContainer => Color.FromRgb(255, 255, 255);
        public Color Secondary => Color.FromRgb(48, 70, 78);
        public Color OnSecondary => Color.FromRgb(255, 255, 255);
        public Color SecondaryContainer => Color.FromRgb(98, 120, 129);
        public Color OnSecondaryContainer => Color.FromRgb(255, 255, 255);
        public Color Tertiary => Color.FromRgb(62, 64, 97);
        public Color OnTertiary => Color.FromRgb(255, 255, 255);
        public Color TertiaryContainer => Color.FromRgb(112, 114, 149);
        public Color OnTertiaryContainer => Color.FromRgb(255, 255, 255);
        public Color Error => Color.FromRgb(140, 0, 9);
        public Color OnError => Color.FromRgb(255, 255, 255);
        public Color ErrorContainer => Color.FromRgb(218, 52, 46);
        public Color OnErrorContainer => Color.FromRgb(255, 255, 255);
        public Color Background => Color.FromRgb(245, 250, 253);
        public Color OnBackground => Color.FromRgb(23, 28, 31);
        public Color Surface => Color.FromRgb(245, 250, 253);
        public Color OnSurface => Color.FromRgb(23, 28, 31);
        public Color SurfaceVariant => Color.FromRgb(219, 228, 232);
        public Color OnSurfaceVariant => Color.FromRgb(60, 68, 72);
        public Color Outline => Color.FromRgb(88, 96, 100);
        public Color OutlineVariant => Color.FromRgb(116, 124, 128);
        public Color Shadow => Color.FromRgb(0, 0, 0);
        public Color Scrim => Color.FromRgb(0, 0, 0);
        public Color InverseSurface => Color.FromRgb(44, 49, 52);
        public Color InverseOnSurface => Color.FromRgb(237, 241, 244);
        public Color InversePrimary => Color.FromRgb(136, 209, 235);
    }
}