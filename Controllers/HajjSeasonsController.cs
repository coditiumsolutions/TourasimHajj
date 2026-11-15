using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class HajjSeasonsController : Controller
{
    private readonly IHajjService _hajj;

    public HajjSeasonsController(IHajjService hajj) => _hajj = hajj;

    public async Task<IActionResult> Index(bool includeInactive, CancellationToken cancellationToken)
    {
        var seasons = await _hajj.GetSeasonsAsync(includeInactive, cancellationToken);
        return View(new HajjSeasonListViewModel { Seasons = seasons, IncludeInactive = includeInactive });
    }

    public IActionResult Create() => View(new HajjSeasonInputModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HajjSeasonInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.CreateSeasonAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not create season."); return View(model); }
        TempData["Success"] = "Hajj season created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var season = await _hajj.GetSeasonByIdAsync(id, cancellationToken);
        if (season is null) return NotFound();
        var model = new HajjSeasonInputModel
        {
            HajjSeasonId = season.HajjSeasonId,
            SeasonName = season.SeasonName,
            HajjYear = season.HajjYear,
            RegistrationStartDate = season.RegistrationStartDate,
            RegistrationEndDate = season.RegistrationEndDate,
            DepartureStartDate = season.DepartureStartDate,
            DepartureEndDate = season.DepartureEndDate,
            Notes = season.Notes,
            IsActive = season.IsActive != false
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HajjSeasonInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.UpdateSeasonAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update season."); return View(model); }
        TempData["Success"] = "Hajj season updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var season = await _hajj.GetSeasonByIdAsync(id, cancellationToken);
        if (season is null) return NotFound();
        return View(season);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _hajj.DeleteSeasonAsync(id, cancellationToken);
        TempData["Success"] = "Hajj season removed or deactivated.";
        return RedirectToAction(nameof(Index));
    }
}