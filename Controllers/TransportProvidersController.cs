using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class TransportProvidersController : Controller
{
    private readonly ISupplierService _suppliers;

    public TransportProvidersController(ISupplierService suppliers) => _suppliers = suppliers;

    public async Task<IActionResult> Index(bool includeInactive, CancellationToken cancellationToken)
    {
        var items = await _suppliers.GetTransportProvidersAsync(includeInactive, cancellationToken);
        return View(new TransportProviderListViewModel { Providers = items, IncludeInactive = includeInactive });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _suppliers.GetTransportProviderFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransportProviderInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetTransportProviderFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.CreateTransportProviderAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Transport provider created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetTransportProviderByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new TransportProviderInputModel
        {
            TransportProviderId = entity.TransportProviderId,
            SupplierId = entity.SupplierId,
            ProviderName = entity.ProviderName,
            VehicleType = entity.VehicleType,
            VehicleCapacity = entity.VehicleCapacity,
            ContactNo = entity.ContactNo,
            IsActive = entity.IsActive != false
        };
        model = await _suppliers.GetTransportProviderFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TransportProviderInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetTransportProviderFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.UpdateTransportProviderAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Transport provider updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetTransportProviderByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _suppliers.DeleteTransportProviderAsync(id, cancellationToken);
        TempData["Success"] = "Transport provider deactivated.";
        return RedirectToAction(nameof(Index));
    }
}