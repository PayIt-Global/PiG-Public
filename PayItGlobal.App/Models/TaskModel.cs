using PayItGlobal.App.Resources.Styles;

namespace PayItGlobal.App.Models;

record TaskModel(string Title, Color BackgroundColor, Color CircleColor)
{
    public static TaskModel[] All => new[] 
    {
        new TaskModel("Aenean etiam ipsum sed nis...", ThemeBrushes.Pink10, ThemeBrushes.Pink20),
        new TaskModel("Massa malesuada id potent...", ThemeBrushes.Green10, ThemeBrushes.Green20),
        new TaskModel("Mi quis vitae augue.", ThemeBrushes.Purple40, ThemeBrushes.Purple30),
    };
}
