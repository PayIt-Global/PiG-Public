using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.App.Resources.Styles
{
    static class ThemeBrushes
    {
        public Color Background => Color.FromArgb("#F7F6FA");
        public Color White => Color.FromArgb("#FFFFFF");

        public Color Dark => Color.FromArgb("#292932");
        public Color Grey100 => Color.FromArgb("#95959D");
        public Color Danger => Color.FromArgb("#FF8B77");
        public Color Purple10 => Color.FromArgb("#7455F6");
        public Color Purple20 => Color.FromArgb("#A5A7FF");
        public Color Purple30 => Color.FromArgb("#B9AAFA");
        public Color Purple40 => Color.FromArgb("#E3DDFD");
        public Color Purple50 => Color.FromArgb("#F1EEFE");

        public Color Green => Color.FromArgb("#43E2C6");
        public Color Pink10 => Color.FromArgb("#FFE1D6");
        public Color Pink20 => Color.FromArgb("#FF8B77");
        public Color Green10 => Color.FromArgb("#D9F9F4");
        public Color Green20 => Color.FromArgb("#43E2C6");

        public Color Grey => Color.FromArgb("#F7F6FA");

        public Color Grey20 => Color.FromArgb("#EAEAEB");
        public Color Grey50 => Color.FromArgb("#CACACE");


        //Shadows
        public Color DarkShadow => Color.FromRgba(41, 41, 50, (int)(0.1 * 255));
        public Color DarkShadow10 => Color.FromRgba(72, 65, 104, (int)(0.1 * 255));
    }
}
