using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class SuppliersController : Controller
{
    private readonly ISupplierService _suppliers;

    public SuppliersController(ISupplierService suppliers) => _suppliers = suppliers;

    public async Task<IActionResult> Index(string? typeFilter, bool includeInactive, CancellationToken cancellationToken)
    {
        var items = await _suppliers.GetSuppliersAsync(typeFilter, includeInactive, cancellationToken);
        return View(new SupplierListViewModel { Suppliers = items, TypeFilter = typeFilter, IncludeInactive = includeInactive });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _suppliers.GetSupplierFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetSupplierFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.CreateSupplierAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Supplier created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetSupplierByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new SupplierInputModel
        {
            SupplierId = entity.SupplierId,
            SupplierName = entity.SupplierName,
            SupplierType = entity.SupplierType,
            ContactPerson = entity.ContactPerson,
            MobileNo = entity.MobileNo,
            Email = entity.Email,
            CountryId = entity.CountryId,
            CityId = entity.CityId,
            Address = entity.Address,
            IsActive = entity.IsActive != false
        };
        model = await _suppliers.GetSupplierFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierInputModel model, CancellationToken cancellationToken)
    {
        model = await _suppliers.GetSupplierFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _suppliers.UpdateSupplierAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Supplier updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _suppliers.GetSupplierByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _suppliers.DeleteSupplierAsync(id, cancellationToken);
        TempData["Success"] = "Supplier deactivated.";
        return RedirectToAction(nameof(Index));
    }
}