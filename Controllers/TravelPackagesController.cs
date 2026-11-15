using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class TravelPackagesController : Controller
{
    private readonly IPackageService _service;
    public TravelPackagesController(IPackageService service) => _service = service;

    public async Task<IActionResult> Index(string? q, string? type, int page = 1)
    {
        ViewData["Title"] = string.IsNullOrWhiteSpace(type) ? "Travel Packages" : $"{type} Packages";
        var (items, total) = await _service.GetPagedAsync(q, type, page, 10);
        ViewBag.Q = q; ViewBag.Type = type; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "New Package";
        return View(new TravelPackage { IsActive = true, CurrencyCode = "PKR", PackageType = "Tourism" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveAsync(model);
        TempData["Success"] = "Package created.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Package";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBasic([Bind(Prefix = "Package")] TravelPackage model)
    {
        await _service.SaveAsync(model);
        TempData["Success"] = "Package updated.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDeparture(PackageDeparture model)
    {
        if (model.AvailableSeats == null) model.AvailableSeats = model.TotalSeats;
        await _service.SaveDepartureAsync(model);
        TempData["Success"] = "Departure saved.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItinerary(PackageItinerary model)
    {
        await _service.SaveItineraryAsync(model);
        TempData["Success"] = "Itinerary day saved.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Package Details";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteAsync(id);
        TempData["Success"] = "Package deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
