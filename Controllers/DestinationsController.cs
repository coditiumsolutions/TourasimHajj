using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class DestinationsController : Controller
{
    private readonly IMasterDataService _service;
    public DestinationsController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, string? search, int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;
        var term = q ?? search;
        var query = _service.GetDestinationsQuery(term);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        ViewBag.Q = term;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.PageSize = pageSize;
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewBag.CityId = new SelectList(await _service.GetActiveCitiesListAsync(cancellationToken: cancellationToken), nameof(City.CityId), nameof(City.CityName));
        return View(new Destination { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Destination model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CityId = new SelectList(await _service.GetActiveCitiesListAsync(cancellationToken: cancellationToken), nameof(City.CityId), nameof(City.CityName), model.CityId);
            return View(model);
        }
        await _service.CreateDestinationAsync(model, cancellationToken);
        TempData["Success"] = "Destination saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetDestinationByIdAsync(id, includeCity: true, cancellationToken);
        if (model == null) return NotFound();
        ViewBag.CityId = new SelectList(await _service.GetActiveCitiesListAsync(cancellationToken: cancellationToken), nameof(City.CityId), nameof(City.CityName), model.CityId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Destination model, CancellationToken cancellationToken)
    {
        if (id != model.DestinationId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.CityId = new SelectList(await _service.GetActiveCitiesListAsync(cancellationToken: cancellationToken), nameof(City.CityId), nameof(City.CityName), model.CityId);
            return View(model);
        }
        await _service.UpdateDestinationAsync(model, cancellationToken);
        TempData["Success"] = "Destination updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetDestinationByIdAsync(id, includeCity: true, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetDestinationByIdAsync(id, includeCity: true, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _service.SoftDeleteDestinationAsync(id, cancellationToken);
        TempData["Success"] = "Destination deactivated.";
        return RedirectToAction(nameof(Index));
    }
}