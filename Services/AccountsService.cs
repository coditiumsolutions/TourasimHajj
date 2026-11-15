using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class AccountsService : IAccountsService
{
    private readonly ApplicationDbContext _db;

    public AccountsService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Payment>> GetPaymentsAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().Include(p => p.Booking).ThenInclude(b => b.Customer).AsQueryable();
        if (from.HasValue)
            query = query.Where(p => p.PaymentDate >= from.Value.ToDateTime(TimeOnly.MinValue));
        if (to.HasValue)
            query = query.Where(p => p.PaymentDate <= to.Value.ToDateTime(TimeOnly.MaxValue));
        return await query.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.PaymentId).ToListAsync(cancellationToken);
    }

    public Task<Payment?> GetPaymentByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.PaymentId == id, cancellationToken);

    public async Task<PaymentInputModel> GetPaymentFormOptionsAsync(PaymentInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new PaymentInputModel();
        model.BookingOptions = await _db.Bookings.AsNoTracking().Where(b => b.BookingStatus != "Cancelled").OrderByDescending(b => b.BookingId).Take(300).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreatePaymentAsync(PaymentInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Payment amount must be greater than zero.");
        var entity = MapPayment(model, new Payment());
        _db.Payments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateBookingPaymentTotalsAsync(entity.BookingId, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdatePaymentAsync(int id, PaymentInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Payment amount must be greater than zero.");
        var entity = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentId == id, cancellationToken);
        if (entity is null) return (false, "Payment not found.");
        var oldBookingId = entity.BookingId;
        MapPayment(model, entity);
        entity.BookingId = model.BookingId;
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateBookingPaymentTotalsAsync(oldBookingId, cancellationToken);
        if (oldBookingId != model.BookingId)
            await RecalculateBookingPaymentTotalsAsync(model.BookingId, cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<Refund>> GetRefundsAsync(CancellationToken cancellationToken = default) =>
        await _db.Refunds.AsNoTracking().Include(r => r.Booking).Include(r => r.Payment).OrderByDescending(r => r.RefundId).ToListAsync(cancellationToken);

    public Task<Refund?> GetRefundByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Refunds.Include(r => r.Booking).Include(r => r.Payment).FirstOrDefaultAsync(r => r.RefundId == id, cancellationToken);

    public async Task<RefundInputModel> GetRefundFormOptionsAsync(RefundInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new RefundInputModel();
        model.BookingOptions = await _db.Bookings.AsNoTracking().OrderByDescending(b => b.BookingId).Take(300).ToListAsync(cancellationToken);
        model.PaymentOptions = await _db.Payments.AsNoTracking().OrderByDescending(p => p.PaymentId).Take(300).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateRefundAsync(RefundInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.RefundAmount <= 0) return (false, "Refund amount must be greater than zero.");
        var entity = MapRefund(model, new Refund());
        _db.Refunds.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateBookingPaymentTotalsAsync(entity.BookingId, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateRefundAsync(int id, RefundInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.RefundAmount <= 0) return (false, "Refund amount must be greater than zero.");
        var entity = await _db.Refunds.FirstOrDefaultAsync(r => r.RefundId == id, cancellationToken);
        if (entity is null) return (false, "Refund not found.");
        var oldBookingId = entity.BookingId;
        MapRefund(model, entity);
        entity.BookingId = model.BookingId;
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateBookingPaymentTotalsAsync(oldBookingId, cancellationToken);
        if (oldBookingId != model.BookingId)
            await RecalculateBookingPaymentTotalsAsync(model.BookingId, cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<Expense>> GetExpensesAsync(CancellationToken cancellationToken = default) =>
        await _db.Expenses.AsNoTracking().Include(e => e.Booking).Include(e => e.Supplier).OrderByDescending(e => e.ExpenseDate).ToListAsync(cancellationToken);

    public Task<Expense?> GetExpenseByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Expenses.Include(e => e.Booking).Include(e => e.Supplier).FirstOrDefaultAsync(e => e.ExpenseId == id, cancellationToken);

    public async Task<ExpenseInputModel> GetExpenseFormOptionsAsync(ExpenseInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new ExpenseInputModel();
        model.BookingOptions = await _db.Bookings.AsNoTracking().OrderByDescending(b => b.BookingId).Take(200).ToListAsync(cancellationToken);
        model.SupplierOptions = await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateExpenseAsync(ExpenseInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Expense amount must be greater than zero.");
        _db.Expenses.Add(MapExpense(model, new Expense()));
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateExpenseAsync(int id, ExpenseInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Expense amount must be greater than zero.");
        var entity = await _db.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id, cancellationToken);
        if (entity is null) return (false, "Expense not found.");
        MapExpense(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteExpenseAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id, cancellationToken);
        if (entity is null) return (false, "Expense not found.");
        _db.Expenses.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<SupplierPayment>> GetSupplierPaymentsAsync(CancellationToken cancellationToken = default) =>
        await _db.SupplierPayments.AsNoTracking().Include(sp => sp.Supplier).Include(sp => sp.Booking).OrderByDescending(sp => sp.PaymentDate).ToListAsync(cancellationToken);

    public Task<SupplierPayment?> GetSupplierPaymentByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.SupplierPayments.Include(sp => sp.Supplier).Include(sp => sp.Booking).FirstOrDefaultAsync(sp => sp.SupplierPaymentId == id, cancellationToken);

    public async Task<SupplierPaymentInputModel> GetSupplierPaymentFormOptionsAsync(SupplierPaymentInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new SupplierPaymentInputModel();
        model.SupplierOptions = await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
        model.BookingOptions = await _db.Bookings.AsNoTracking().OrderByDescending(b => b.BookingId).Take(200).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateSupplierPaymentAsync(SupplierPaymentInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Amount must be greater than zero.");
        _db.SupplierPayments.Add(MapSupplierPayment(model, new SupplierPayment()));
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateSupplierPaymentAsync(int id, SupplierPaymentInputModel model, CancellationToken cancellationToken = default)
    {
        if (model.Amount <= 0) return (false, "Amount must be greater than zero.");
        var entity = await _db.SupplierPayments.FirstOrDefaultAsync(sp => sp.SupplierPaymentId == id, cancellationToken);
        if (entity is null) return (false, "Supplier payment not found.");
        MapSupplierPayment(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteSupplierPaymentAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SupplierPayments.FirstOrDefaultAsync(sp => sp.SupplierPaymentId == id, cancellationToken);
        if (entity is null) return (false, "Supplier payment not found.");
        _db.SupplierPayments.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<CustomerLedgerRow>> GetCustomerLedgerAsync(LedgerFilterModel filter, CancellationToken cancellationToken = default)
    {
        if (!filter.CustomerId.HasValue) return Array.Empty<CustomerLedgerRow>();
        var customerId = filter.CustomerId.Value;
        var bookings = await _db.Bookings.AsNoTracking().Where(b => b.CustomerId == customerId).ToListAsync(cancellationToken);
        var bookingIds = bookings.Select(b => b.BookingId).ToList();
        var rows = new List<CustomerLedgerRow>();
        foreach (var booking in bookings)
        {
            if (filter.FromDate.HasValue && booking.BookingDate.HasValue && DateOnly.FromDateTime(booking.BookingDate.Value) < filter.FromDate) continue;
            if (filter.ToDate.HasValue && booking.BookingDate.HasValue && DateOnly.FromDateTime(booking.BookingDate.Value) > filter.ToDate) continue;
            rows.Add(new CustomerLedgerRow { TransactionDate = booking.BookingDate, TransactionType = "Booking", Reference = booking.BookingNo, Debit = booking.NetAmount ?? 0m, Credit = 0m });
        }
        foreach (var payment in await _db.Payments.AsNoTracking().Include(p => p.Booking).Where(p => bookingIds.Contains(p.BookingId)).ToListAsync(cancellationToken))
        {
            var pDate = payment.PaymentDate.HasValue ? DateOnly.FromDateTime(payment.PaymentDate.Value) : (DateOnly?)null;
            if (filter.FromDate.HasValue && pDate < filter.FromDate) continue;
            if (filter.ToDate.HasValue && pDate > filter.ToDate) continue;
            rows.Add(new CustomerLedgerRow { TransactionDate = payment.PaymentDate, TransactionType = "Payment", Reference = payment.ReferenceNo ?? payment.Booking.BookingNo, Debit = 0m, Credit = payment.Amount });
        }
        foreach (var refund in await _db.Refunds.AsNoTracking().Include(r => r.Booking).Where(r => bookingIds.Contains(r.BookingId) && (r.RefundStatus == null || r.RefundStatus != "Cancelled")).ToListAsync(cancellationToken))
        {
            if (filter.FromDate.HasValue && refund.RefundDate < filter.FromDate) continue;
            if (filter.ToDate.HasValue && refund.RefundDate > filter.ToDate) continue;
            rows.Add(new CustomerLedgerRow { TransactionDate = refund.RefundDate?.ToDateTime(TimeOnly.MinValue), TransactionType = "Refund", Reference = refund.RefundReason, Debit = refund.RefundAmount, Credit = 0m });
        }
        rows = rows.OrderBy(r => r.TransactionDate ?? DateTime.MinValue).ToList();
        decimal balance = 0m;
        foreach (var row in rows) { balance += row.Debit - row.Credit; row.Balance = balance; }
        return rows;
    }

    public async Task<IReadOnlyList<SupplierLedgerRow>> GetSupplierLedgerAsync(LedgerFilterModel filter, CancellationToken cancellationToken = default)
    {
        if (!filter.SupplierId.HasValue) return Array.Empty<SupplierLedgerRow>();
        var supplierId = filter.SupplierId.Value;
        var rows = new List<SupplierLedgerRow>();
        foreach (var expense in await _db.Expenses.AsNoTracking().Where(e => e.SupplierId == supplierId).ToListAsync(cancellationToken))
        {
            if (filter.FromDate.HasValue && expense.ExpenseDate < filter.FromDate) continue;
            if (filter.ToDate.HasValue && expense.ExpenseDate > filter.ToDate) continue;
            rows.Add(new SupplierLedgerRow { TransactionDate = expense.ExpenseDate, TransactionType = "Expense", Reference = expense.ReferenceNo ?? expense.ExpenseCategory, Debit = expense.Amount, Credit = 0m });
        }
        foreach (var payment in await _db.SupplierPayments.AsNoTracking().Where(sp => sp.SupplierId == supplierId).ToListAsync(cancellationToken))
        {
            if (filter.FromDate.HasValue && payment.PaymentDate < filter.FromDate) continue;
            if (filter.ToDate.HasValue && payment.PaymentDate > filter.ToDate) continue;
            rows.Add(new SupplierLedgerRow { TransactionDate = payment.PaymentDate, TransactionType = "Payment", Reference = payment.ReferenceNo, Debit = 0m, Credit = payment.Amount });
        }
        rows = rows.OrderBy(r => r.TransactionDate).ToList();
        decimal balance = 0m;
        foreach (var row in rows) { balance += row.Debit - row.Credit; row.Balance = balance; }
        return rows;
    }

    public async Task<ProfitLossSummaryViewModel> GetProfitLossSummaryAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var payments = _db.Payments.AsNoTracking().AsQueryable();
        var refunds = _db.Refunds.AsNoTracking().Where(r => r.RefundStatus == null || r.RefundStatus != "Cancelled");
        var expenses = _db.Expenses.AsNoTracking();
        var supplierPayments = _db.SupplierPayments.AsNoTracking();
        if (from.HasValue)
        {
            payments = payments.Where(p => p.PaymentDate >= from.Value.ToDateTime(TimeOnly.MinValue));
            refunds = refunds.Where(r => r.RefundDate >= from);
            expenses = expenses.Where(e => e.ExpenseDate >= from);
            supplierPayments = supplierPayments.Where(sp => sp.PaymentDate >= from);
        }
        if (to.HasValue)
        {
            payments = payments.Where(p => p.PaymentDate <= to.Value.ToDateTime(TimeOnly.MaxValue));
            refunds = refunds.Where(r => r.RefundDate <= to);
            expenses = expenses.Where(e => e.ExpenseDate <= to);
            supplierPayments = supplierPayments.Where(sp => sp.PaymentDate <= to);
        }
        var totalCollections = await payments.SumAsync(p => p.Amount, cancellationToken);
        var totalRefunds = await refunds.SumAsync(r => r.RefundAmount, cancellationToken);
        var totalExpenses = await expenses.SumAsync(e => e.Amount, cancellationToken);
        var totalSupplierPayments = await supplierPayments.SumAsync(sp => sp.Amount, cancellationToken);
        var netRevenue = totalCollections - totalRefunds;
        return new ProfitLossSummaryViewModel
        {
            FromDate = from,
            ToDate = to,
            TotalCollections = totalCollections,
            TotalRefunds = totalRefunds,
            NetRevenue = netRevenue,
            TotalExpenses = totalExpenses,
            TotalSupplierPayments = totalSupplierPayments,
            GrossProfit = netRevenue - totalExpenses - totalSupplierPayments
        };
    }

    public async Task RecalculateBookingPaymentTotalsAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken);
        if (booking is null) return;
        var totalPaid = await _db.Payments.Where(p => p.BookingId == bookingId).SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;
        var totalRefunded = await _db.Refunds.Where(r => r.BookingId == bookingId && (r.RefundStatus == null || r.RefundStatus != "Cancelled")).SumAsync(r => (decimal?)r.RefundAmount, cancellationToken) ?? 0m;
        var netPaid = Math.Max(0, totalPaid - totalRefunded);
        booking.PaidAmount = netPaid;
        var netAmount = booking.NetAmount ?? 0m;
        booking.BalanceAmount = Math.Max(0, netAmount - netPaid);
        booking.PaymentStatus = netPaid <= 0 ? "Unpaid" : booking.BalanceAmount <= 0 ? "Paid" : "Partial";
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static Payment MapPayment(PaymentInputModel model, Payment entity)
    {
        entity.BookingId = model.BookingId;
        entity.PaymentDate = model.PaymentDate ?? DateTime.UtcNow;
        entity.PaymentType = string.IsNullOrWhiteSpace(model.PaymentType) ? "Receipt" : model.PaymentType.Trim();
        entity.PaymentMethod = model.PaymentMethod?.Trim();
        entity.ReferenceNo = model.ReferenceNo?.Trim();
        entity.Amount = model.Amount;
        entity.CurrencyCode = string.IsNullOrWhiteSpace(model.CurrencyCode) ? "PKR" : model.CurrencyCode.Trim();
        entity.ReceivedBy = model.ReceivedBy?.Trim();
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }

    private static Refund MapRefund(RefundInputModel model, Refund entity)
    {
        entity.BookingId = model.BookingId;
        entity.PaymentId = model.PaymentId;
        entity.RefundDate = model.RefundDate;
        entity.RefundAmount = model.RefundAmount;
        entity.RefundReason = model.RefundReason?.Trim();
        entity.RefundStatus = string.IsNullOrWhiteSpace(model.RefundStatus) ? "Completed" : model.RefundStatus.Trim();
        entity.PaymentMethod = model.PaymentMethod?.Trim();
        return entity;
    }

    private static Expense MapExpense(ExpenseInputModel model, Expense entity)
    {
        entity.BookingId = model.BookingId;
        entity.SupplierId = model.SupplierId;
        entity.ExpenseDate = model.ExpenseDate;
        entity.ExpenseCategory = model.ExpenseCategory?.Trim();
        entity.Description = model.Description?.Trim();
        entity.Amount = model.Amount;
        entity.PaymentMethod = model.PaymentMethod?.Trim();
        entity.ReferenceNo = model.ReferenceNo?.Trim();
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }

    private static SupplierPayment MapSupplierPayment(SupplierPaymentInputModel model, SupplierPayment entity)
    {
        entity.SupplierId = model.SupplierId;
        entity.BookingId = model.BookingId;
        entity.PaymentDate = model.PaymentDate;
        entity.PaymentMethod = model.PaymentMethod?.Trim();
        entity.ReferenceNo = model.ReferenceNo?.Trim();
        entity.Amount = model.Amount;
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }
}