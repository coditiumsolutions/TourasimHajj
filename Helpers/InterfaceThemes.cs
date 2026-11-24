namespace TourismHajj.Helpers;

public record InterfaceThemeOption(
    string Id,
    string Name,
    string PrimaryColor,
    string SecondaryColor);

public static class InterfaceThemes
{
    public const string ManagementConfigKey = "ManagementInterfaceTheme";
    public const string CustomerConfigKey = "CustomerInterfaceTheme";
    public const string DefaultThemeId = "navy-amber";

    public static IReadOnlyList<InterfaceThemeOption> All { get; } =
    [
        new("navy-amber", "Navy & Amber", "#102944", "#ffb20b"),
        new("ocean-sky", "Ocean & Sky", "#00537A", "#A8E8F9"),
        new("deep-teal-gold", "Deep Teal & Gold", "#013C58", "#F5A201"),
        new("midnight-sun", "Midnight & Sun", "#0A174E", "#F5D042"),
        new("charcoal-yellow", "Charcoal & Yellow", "#101820", "#FEE715"),
        new("ink-mint", "Ink & Mint", "#00203F", "#ADEFD1"),
        new("black-neon", "Black & Neon Green", "#0A0A0A", "#39FF14"),
        new("forest-leaf", "Forest & Leaf", "#2C5F2D", "#97BC62"),
        new("espresso-coral", "Espresso & Coral", "#2D2926", "#ED6F63")
    ];

    public static InterfaceThemeOption GetById(string? id) =>
        All.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase))
        ?? All.First(t => t.Id == DefaultThemeId)
        ?? All[0];
}
