using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class CountriesController : Controller
{
    private readonly IMasterDataService _service;
    public CountriesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? search, int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;
        var query = _service.GetCountriesQuery(search);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var model = new PagedResult<Country>
        {
            Items = items,
            PageIndex = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Search = search
        };
        return View(model);
    }

    public IActionResult Create() => View(new Country { IsActive = true });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Country model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.CreateCountryAsync(model, cancellationToken);
        TempData["Success"] = "Country saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCountryByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Country model, CancellationToken cancellationToken)
    {
        if (id != model.CountryId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateCountryAsync(model, cancellationToken);
        TempData["Success"] = "Country updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCountryByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCountryByIdAsync(id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _service.SoftDeleteCountryAsync(id, cancellationToken);
        TempData["Success"] = "Country deactivated.";
        return RedirectToAction(nameof(Index));
    }
}