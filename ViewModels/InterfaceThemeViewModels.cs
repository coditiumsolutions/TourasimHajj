using TourismHajj.Helpers;

namespace TourismHajj.ViewModels;

public class InterfaceThemePickerViewModel
{
    public string Scope { get; set; } = "portal";
    public string PageTitle { get; set; } = "Interface";
    public string PageSubtitle { get; set; } = string.Empty;
    public string SelectedThemeId { get; set; } = InterfaceThemes.DefaultThemeId;
    public IReadOnlyList<InterfaceThemeOption> Themes { get; set; } = Array.Empty<InterfaceThemeOption>();
}
