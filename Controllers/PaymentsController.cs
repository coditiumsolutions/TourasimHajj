using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IAccountsService _accounts;

    public PaymentsController(IAccountsService accounts) => _accounts = accounts;

    public async Task<IActionResult> Index(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var payments = await _accounts.GetPaymentsAsync(fromDate, toDate, cancellationToken);
        return View(new PaymentListViewModel { Payments = payments, FromDate = fromDate, ToDate = toDate });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _accounts.GetPaymentFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetPaymentFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.CreatePaymentAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save payment."); return View(model); }
        TempData["Success"] = "Payment recorded.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetPaymentByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new PaymentInputModel
        {
            PaymentId = entity.PaymentId,
            BookingId = entity.BookingId,
            PaymentDate = entity.PaymentDate,
            PaymentType = entity.PaymentType,
            PaymentMethod = entity.PaymentMethod,
            ReferenceNo = entity.ReferenceNo,
            Amount = entity.Amount,
            CurrencyCode = entity.CurrencyCode,
            ReceivedBy = entity.ReceivedBy,
            Remarks = entity.Remarks
        };
        model = await _accounts.GetPaymentFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetPaymentFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.UpdatePaymentAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update payment."); return View(model); }
        TempData["Success"] = "Payment updated.";
        return RedirectToAction(nameof(Index));
    }
}