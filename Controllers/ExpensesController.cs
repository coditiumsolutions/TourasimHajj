using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IAccountsService _accounts;

    public ExpensesController(IAccountsService accounts) => _accounts = accounts;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await _accounts.GetExpensesAsync(cancellationToken);
        return View(new ExpenseListViewModel { Expenses = items });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _accounts.GetExpenseFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetExpenseFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.CreateExpenseAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save expense."); return View(model); }
        TempData["Success"] = "Expense recorded.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetExpenseByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new ExpenseInputModel
        {
            ExpenseId = entity.ExpenseId,
            BookingId = entity.BookingId,
            SupplierId = entity.SupplierId,
            ExpenseDate = entity.ExpenseDate,
            ExpenseCategory = entity.ExpenseCategory,
            Description = entity.Description,
            Amount = entity.Amount,
            PaymentMethod = entity.PaymentMethod,
            ReferenceNo = entity.ReferenceNo,
            Remarks = entity.Remarks
        };
        model = await _accounts.GetExpenseFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseInputModel model, CancellationToken cancellationToken)
    {
        model = await _accounts.GetExpenseFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _accounts.UpdateExpenseAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update expense."); return View(model); }
        TempData["Success"] = "Expense updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _accounts.GetExpenseByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _accounts.DeleteExpenseAsync(id, cancellationToken);
        TempData["Success"] = "Expense removed.";
        return RedirectToAction(nameof(Index));
    }
}