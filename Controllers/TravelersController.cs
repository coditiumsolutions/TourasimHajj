using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class TravelersController : Controller
{
    private readonly ITravelerService _service;
    private readonly IMasterDataService _masters;
    private readonly ApplicationDbContext _db;

    public TravelersController(ITravelerService service, IMasterDataService masters, ApplicationDbContext db)
    {
        _service = service; _masters = masters; _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? customerId, int page = 1)
    {
        ViewData["Title"] = "Travelers";
        var (items, total) = await _service.GetPagedAsync(q, customerId, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> PassportAlerts()
    {
        ViewData["Title"] = "Passport Management";
        return View(await _service.GetPassportAlertsAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Traveler Details";
        var model = await _service.GetByIdAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create(int? customerId)
    {
        ViewData["Title"] = "New Traveler";
        await LoadLookups(customerId);
        return View(new Traveler { CustomerId = customerId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Traveler model)
    {
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Traveler";
        await LoadLookups(model.CustomerId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Traveler model)
    {
        if (id != model.TravelerId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Traveler deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups(int? customerId)
    {
        ViewBag.Customers = new SelectList(await _db.Customers.Where(c => c.IsActive != false).OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", customerId);
        ViewBag.Countries = new SelectList(await _masters.GetActiveCountriesListAsync(), nameof(Country.CountryId), nameof(Country.CountryName));
    }
}
