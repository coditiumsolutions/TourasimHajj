using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class SupplierPaymentsController : Controller
{
    private readonly IAccountsService _accounts;

    public SupplierPaymentsController(IAccountsService accounts) => _accounts = accounts;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await _accounts.GetSupplierPaymentsAsync(cancellationToken);
        return View(new SupplierPaymentListViewModel { Payments = items });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _accounts.GetSupplierPaymentFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierPaymentInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetSupplierPaymentFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.CreateSupplierPaymentAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Supplier payment recorded.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetSupplierPaymentByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new SupplierPaymentInputModel
        {
            SupplierPaymentId = entity.SupplierPaymentId,
            SupplierId = entity.SupplierId,
            BookingId = entity.BookingId,
            PaymentDate = entity.PaymentDate,
            PaymentMethod = entity.PaymentMethod,
            ReferenceNo = entity.ReferenceNo,
            Amount = entity.Amount,
            Remarks = entity.Remarks
        };
        model = await _accounts.GetSupplierPaymentFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierPaymentInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetSupplierPaymentFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.UpdateSupplierPaymentAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update."); return View(model); }
        TempData["Success"] = "Supplier payment updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetSupplierPaymentByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _accounts.DeleteSupplierPaymentAsync(id, cancellationToken);
        TempData["Success"] = "Supplier payment removed.";
        return RedirectToAction(nameof(Index));
    }
}