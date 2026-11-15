using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class RefundsController : Controller
{
    private readonly IAccountsService _accounts;

    public RefundsController(IAccountsService accounts) => _accounts = accounts;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var refunds = await _accounts.GetRefundsAsync(cancellationToken);
        return View(new RefundListViewModel { Refunds = refunds });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _accounts.GetRefundFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RefundInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetRefundFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.CreateRefundAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save refund."); return View(model); }
        TempData["Success"] = "Refund recorded.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetRefundByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new RefundInputModel
        {
            RefundId = entity.RefundId,
            BookingId = entity.BookingId,
            PaymentId = entity.PaymentId,
            RefundDate = entity.RefundDate,
            RefundAmount = entity.RefundAmount,
            RefundReason = entity.RefundReason,
            RefundStatus = entity.RefundStatus,
            PaymentMethod = entity.PaymentMethod
        };
        model = await _accounts.GetRefundFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RefundInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetRefundFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.UpdateRefundAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update refund."); return View(model); }
        TempData["Success"] = "Refund updated.";
        return RedirectToAction(nameof(Index));
    }
}