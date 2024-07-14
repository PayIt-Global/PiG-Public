using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.App.Pages.Components
{
    public class MainMenuStateService
    {
        public MainMenuState CurrentState { get; private set; } = new MainMenuState();

        // Methods to update the state as needed
        public void UpdateTranslationX(double value)
        {
            CurrentState.TranslationX = value;
            // Notify observers of the change if necessary
        }

        // Additional methods to update other properties
    }
    public class SideMenuStateService
    {
        public SideMenuState CurrentState { get; private set; } = new SideMenuState();

        // Methods to update the state as needed
        public void UpdateTranslationX(double value)
        {
            CurrentState.TranslationX = value;
            // Notify observers of the change if necessary
        }

        // Additional methods to update other properties
    }
}
