using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class ServicesController : Controller
{
    private readonly IMasterDataService _service;
    public ServicesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, string? search, int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;
        var term = q ?? search;
        var query = _service.GetServicesQuery(term);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        ViewBag.Q = term;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.PageSize = pageSize;
        return View(items);
    }

    public IActionResult Create() => View(new Service { IsActive = true });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Service model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.CreateServiceAsync(model, cancellationToken);
        TempData["Success"] = "Service saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetServiceByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Service model, CancellationToken cancellationToken)
    {
        if (id != model.ServiceId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateServiceAsync(model, cancellationToken);
        TempData["Success"] = "Service updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetServiceByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetServiceByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _service.SoftDeleteServiceAsync(id, cancellationToken);
        TempData["Success"] = "Service deactivated.";
        return RedirectToAction(nameof(Index));
    }
}