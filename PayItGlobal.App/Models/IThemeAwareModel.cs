using PayItGlobal.App.Resources.Styles;

namespace PayItGlobal.App.Models;
public interface IThemeAwareModel<T> where T : IThemeAwareModel<T>
{
    T WithThemeColors(IThemeColors themeColors);
}
