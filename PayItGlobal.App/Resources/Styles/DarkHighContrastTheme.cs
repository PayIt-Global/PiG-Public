using Microsoft.Maui.Controls;
using PayItGlobal.App.Themes;

namespace PayItGlobal.App.Resources.Styles
{
    public class DarkHighContrastTheme : IThemeColors
    {
        public Color Primary => Color.FromRgb(246, 252, 255);
        public Color SurfaceTint => Color.FromRgb(136, 209, 235);
        public Color OnPrimary => Color.FromRgb(0, 0, 0);
        public Color PrimaryContainer => Color.FromRgb(140, 213, 240);
        public Color OnPrimaryContainer => Color.FromRgb(0, 0, 0);
        public Color Secondary => Color.FromRgb(246, 252, 255);
        public Color OnSecondary => Color.FromRgb(0, 0, 0);
        public Color SecondaryContainer => Color.FromRgb(183, 206, 216);
        public Color OnSecondaryContainer => Color.FromRgb(0, 0, 0);
        public Color Tertiary => Color.FromRgb(253, 249, 255);
        public Color OnTertiary => Color.FromRgb(0, 0, 0);
        public Color TertiaryContainer => Color.FromRgb(199, 200, 239);
        public Color OnTertiaryContainer => Color.FromRgb(0, 0, 0);
        public Color Error => Color.FromRgb(255, 249, 249);
        public Color OnError => Color.FromRgb(0, 0, 0);
        public Color ErrorContainer => Color.FromRgb(255, 186, 177);
        public Color OnErrorContainer => Color.FromRgb(0, 0, 0);
        public Color Background => Color.FromRgb(15, 20, 22);
        public Color OnBackground => Color.FromRgb(222, 227, 230);
        public Color Surface => Color.FromRgb(15, 20, 22);
        public Color OnSurface => Color.FromRgb(255, 255, 255);
        public Color SurfaceVariant => Color.FromRgb(64, 72, 76);
        public Color OnSurfaceVariant => Color.FromRgb(246, 252, 255);
        public Color Outline => Color.FromRgb(196, 204, 208);
        public Color OutlineVariant => Color.FromRgb(196, 204, 208);
        public Color Shadow => Color.FromRgb(0, 0, 0);
        public Color Scrim => Color.FromRgb(0, 0, 0);
        public Color InverseSurface => Color.FromRgb(222, 227, 230);
        public Color InverseOnSurface => Color.FromRgb(0, 0, 0);
        public Color InversePrimary => Color.FromRgb(0, 46, 59);
        // Add more colors as needed...
    }
}
