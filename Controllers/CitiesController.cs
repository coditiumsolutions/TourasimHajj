using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class CitiesController : Controller
{
    private readonly IMasterDataService _service;
    public CitiesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, string? search, int? countryId, int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;
        var term = q ?? search;
        var query = _service.GetCitiesQuery(term, countryId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesListAsync(cancellationToken), nameof(Country.CountryId), nameof(Country.CountryName), countryId);
        ViewBag.Q = term;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.PageSize = pageSize;
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesListAsync(cancellationToken), nameof(Country.CountryId), nameof(Country.CountryName));
        return View(new City { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(City model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = new SelectList(await _service.GetActiveCountriesListAsync(cancellationToken), nameof(Country.CountryId), nameof(Country.CountryName), model.CountryId);
            return View(model);
        }
        await _service.CreateCityAsync(model, cancellationToken);
        TempData["Success"] = "City saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCityByIdAsync(id, includeCountry: true, cancellationToken);
        if (model == null) return NotFound();
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesListAsync(cancellationToken), nameof(Country.CountryId), nameof(Country.CountryName), model.CountryId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, City model, CancellationToken cancellationToken)
    {
        if (id != model.CityId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = new SelectList(await _service.GetActiveCountriesListAsync(cancellationToken), nameof(Country.CountryId), nameof(Country.CountryName), model.CountryId);
            return View(model);
        }
        await _service.UpdateCityAsync(model, cancellationToken);
        TempData["Success"] = "City updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCityByIdAsync(id, includeCountry: true, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _service.GetCityByIdAsync(id, includeCountry: true, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _service.SoftDeleteCityAsync(id, cancellationToken);
        TempData["Success"] = "City deactivated.";
        return RedirectToAction(nameof(Index));
    }
}