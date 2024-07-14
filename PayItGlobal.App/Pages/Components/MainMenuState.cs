using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages.Components
{
    public interface IMainMenuState
    {
        double TranslationX { get; set; }

        double RotationY { get; set; }

        double MarginLeft { get; set; }

        double MainScale { get; set; }

        double MainOpacity { get; set; }
    }
    public class MainMenuState : IMainMenuState
    {
        public double TranslationX { get; set; } = 220;

        public double RotationY { get; set; } = -12;

        public double MarginLeft { get; set; } = -30.0;

        public double MainScale { get; set; } = 1.0;

        public double MainOpacity { get; set; } = 1.0;
    }
}
