using PayItGlobalApi.App.Themes;

namespace PayItGlobalApi.App.Models;

public record TaskModel(string Title, Color BackgroundColor, Color CircleColor) : IThemeAwareModel<TaskModel>
{
    public TaskModel WithThemeColors(IThemeColors themeColors)
    {
        return this with { BackgroundColor = themeColors.Primary, CircleColor = themeColors.Secondary };
    }
}
