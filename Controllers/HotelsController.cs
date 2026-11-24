using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class HotelsController : Controller
{
    private readonly ISupplierService _suppliers;

    public HotelsController(ISupplierService suppliers) => _suppliers = suppliers;

    public async Task<IActionResult> Index(bool includeInactive, CancellationToken cancellationToken)
    {
        ViewData["Title"] = includeInactive ? "Inactive Hotels" : "All Hotels";
        var hotels = await _suppliers.GetHotelsAsync(includeInactive, cancellationToken);
        return View(new HotelListViewModel { Hotels = hotels, IncludeInactive = includeInactive });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetHotelByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _suppliers.GetHotelFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetHotelFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.CreateHotelAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Hotel created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetHotelByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new HotelInputModel
        {
            HotelId = entity.HotelId,
            SupplierId = entity.SupplierId,
            CityId = entity.CityId,
            HotelName = entity.HotelName,
            StarRating = entity.StarRating,
            Address = entity.Address,
            DistanceFromHaramKm = entity.DistanceFromHaramKm,
            DistanceFromNabawiKm = entity.DistanceFromNabawiKm,
            ContactNo = entity.ContactNo,
            IsActive = entity.IsActive != false
        };
        model = await _suppliers.GetHotelFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HotelInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetHotelFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.UpdateHotelAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Hotel updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetHotelByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _suppliers.DeleteHotelAsync(id, cancellationToken);
        TempData["Success"] = "Hotel deactivated.";
        return RedirectToAction(nameof(Index));
    }
}