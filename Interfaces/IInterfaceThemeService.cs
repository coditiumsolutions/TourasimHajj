using TourismHajj.Helpers;

namespace TourismHajj.Interfaces;

public interface IInterfaceThemeService
{
    Task<InterfaceThemeOption> GetManagementThemeAsync(CancellationToken cancellationToken = default);
    Task<InterfaceThemeOption> GetCustomerThemeAsync(CancellationToken cancellationToken = default);
    Task SetManagementThemeAsync(string themeId, CancellationToken cancellationToken = default);
    Task SetCustomerThemeAsync(string themeId, CancellationToken cancellationToken = default);
}
