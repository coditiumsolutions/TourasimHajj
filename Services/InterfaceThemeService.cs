using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Services;

public class InterfaceThemeService : IInterfaceThemeService
{
    private readonly ApplicationDbContext _db;

    public InterfaceThemeService(ApplicationDbContext db) => _db = db;

    public async Task<InterfaceThemeOption> GetManagementThemeAsync(CancellationToken cancellationToken = default) =>
        InterfaceThemes.GetById(await GetValueAsync(InterfaceThemes.ManagementConfigKey, cancellationToken));

    public async Task<InterfaceThemeOption> GetCustomerThemeAsync(CancellationToken cancellationToken = default) =>
        InterfaceThemes.GetById(await GetValueAsync(InterfaceThemes.CustomerConfigKey, cancellationToken));

    public Task SetManagementThemeAsync(string themeId, CancellationToken cancellationToken = default) =>
        SetValueAsync(InterfaceThemes.ManagementConfigKey, InterfaceThemes.GetById(themeId).Id, cancellationToken);

    public Task SetCustomerThemeAsync(string themeId, CancellationToken cancellationToken = default) =>
        SetValueAsync(InterfaceThemes.CustomerConfigKey, InterfaceThemes.GetById(themeId).Id, cancellationToken);

    private async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken)
    {
        return await _db.AppConfigs.AsNoTracking()
            .Where(c => c.ConfigKey == key && c.IsActive)
            .Select(c => c.ConfigValue)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task SetValueAsync(string key, string value, CancellationToken cancellationToken)
    {
        var item = await _db.AppConfigs.FirstOrDefaultAsync(c => c.ConfigKey == key, cancellationToken);
        var now = DateTime.Now;
        if (item is null)
        {
            _db.AppConfigs.Add(new AppConfig
            {
                ConfigKey = key,
                ConfigValue = value,
                Description = "Selected interface color theme.",
                IsActive = true,
                CreatedDate = now,
                UpdatedDate = now
            });
        }
        else
        {
            item.ConfigValue = value;
            item.IsActive = true;
            item.UpdatedDate = now;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
