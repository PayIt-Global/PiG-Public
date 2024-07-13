using PayItGlobal.App.Resources.Styles;
using System;

namespace PayItGlobal.App.Models
{
    public class ThemeAwareModelFactory<T> where T : IThemeAwareModel<T>, new()
    {
        private readonly IThemeColors _currentTheme;

        public ThemeAwareModelFactory(IThemeColors currentTheme)
        {
            _currentTheme = currentTheme;
        }

        public T CreateModel(Action<T> additionalSetup = null)
        {
            // Create a new instance of T.
            var model = new T();

            // Apply theme colors using the WithThemeColors method, which returns a new instance.
            // Note: This assumes that WithThemeColors does not significantly change the model's state
            // beyond applying theme colors. If additionalSetup is expected to modify the model significantly,
            // consider applying it before WithThemeColors, or adjust this pattern accordingly.
            T themedModel = model.WithThemeColors(_currentTheme);

            // Invoke any additional setup actions on the themed model.
            additionalSetup?.Invoke(themedModel);

            return themedModel;
        }
    }
}
