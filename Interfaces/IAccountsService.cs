using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IAccountsService
{
    Task<IReadOnlyList<Payment>> GetPaymentsAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaymentInputModel> GetPaymentFormOptionsAsync(PaymentInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreatePaymentAsync(PaymentInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdatePaymentAsync(int id, PaymentInputModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Refund>> GetRefundsAsync(CancellationToken cancellationToken = default);
    Task<Refund?> GetRefundByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RefundInputModel> GetRefundFormOptionsAsync(RefundInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateRefundAsync(RefundInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateRefundAsync(int id, RefundInputModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Expense>> GetExpensesAsync(CancellationToken cancellationToken = default);
    Task<Expense?> GetExpenseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ExpenseInputModel> GetExpenseFormOptionsAsync(ExpenseInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateExpenseAsync(ExpenseInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateExpenseAsync(int id, ExpenseInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteExpenseAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplierPayment>> GetSupplierPaymentsAsync(CancellationToken cancellationToken = default);
    Task<SupplierPayment?> GetSupplierPaymentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SupplierPaymentInputModel> GetSupplierPaymentFormOptionsAsync(SupplierPaymentInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateSupplierPaymentAsync(SupplierPaymentInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateSupplierPaymentAsync(int id, SupplierPaymentInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteSupplierPaymentAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerLedgerRow>> GetCustomerLedgerAsync(LedgerFilterModel filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierLedgerRow>> GetSupplierLedgerAsync(LedgerFilterModel filter, CancellationToken cancellationToken = default);
    Task<ProfitLossSummaryViewModel> GetProfitLossSummaryAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);

    Task RecalculateBookingPaymentTotalsAsync(int bookingId, CancellationToken cancellationToken = default);
}