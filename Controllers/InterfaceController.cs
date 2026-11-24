using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class InterfaceController : Controller
{
    private readonly IInterfaceThemeService _themes;

    public InterfaceController(IInterfaceThemeService themes) => _themes = themes;

    [HttpGet]
    public async Task<IActionResult> PortalInterface(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Portal Layout";
        var selected = await _themes.GetManagementThemeAsync(cancellationToken);
        return View("ThemePicker", new InterfaceThemePickerViewModel
        {
            Scope = "portal",
            PageTitle = "Portal Layout",
            PageSubtitle = "Color combination for the Management portal (top navbar, side navbar, and second top navbar).",
            SelectedThemeId = selected.Id,
            Themes = InterfaceThemes.All
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PortalInterface(string themeId, CancellationToken cancellationToken)
    {
        await _themes.SetManagementThemeAsync(themeId, cancellationToken);
        TempData["Success"] = "Portal Layout theme applied.";
        return RedirectToAction(nameof(PortalInterface));
    }

    [HttpGet]
    public async Task<IActionResult> CustomersInterface(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Customer Layout";
        var selected = await _themes.GetCustomerThemeAsync(cancellationToken);
        return View("ThemePicker", new InterfaceThemePickerViewModel
        {
            Scope = "customers",
            PageTitle = "Customer Layout",
            PageSubtitle = "Color combination for the Customers portal (sidebar and top bar).",
            SelectedThemeId = selected.Id,
            Themes = InterfaceThemes.All
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomersInterface(string themeId, CancellationToken cancellationToken)
    {
        await _themes.SetCustomerThemeAsync(themeId, cancellationToken);
        TempData["Success"] = "Customer Layout theme applied.";
        return RedirectToAction(nameof(CustomersInterface));
    }
}
