using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Dashboard";
        var model = await _dashboard.GetDashboardAsync();
        return View(model);
    }
}
