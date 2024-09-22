using PayItGlobalApi.App.Themes;

namespace PayItGlobalApi.App.Models;
public interface IThemeAwareModel<T> where T : IThemeAwareModel<T>
{
    T WithThemeColors(IThemeColors themeColors);
}
