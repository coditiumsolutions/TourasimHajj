using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class PaymentListViewModel
{
    public IReadOnlyList<Payment> Payments { get; set; } = Array.Empty<Payment>();
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}

public class PaymentInputModel
{
    public int PaymentId { get; set; }

    [Required]
    [Display(Name = "Booking")]
    public int BookingId { get; set; }

    [Display(Name = "Payment Date")]
    public DateTime? PaymentDate { get; set; } = DateTime.Today;

    [StringLength(30)]
    [Display(Name = "Payment Type")]
    public string? PaymentType { get; set; } = "Receipt";

    [StringLength(30)]
    [Display(Name = "Method")]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    [Display(Name = "Reference No")]
    public string? ReferenceNo { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; } = "PKR";

    [StringLength(100)]
    [Display(Name = "Received By")]
    public string? ReceivedBy { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public IReadOnlyList<Booking> BookingOptions { get; set; } = Array.Empty<Booking>();
}

public class RefundListViewModel
{
    public IReadOnlyList<Refund> Refunds { get; set; } = Array.Empty<Refund>();
}

public class RefundInputModel
{
    public int RefundId { get; set; }

    [Required]
    [Display(Name = "Booking")]
    public int BookingId { get; set; }

    [Display(Name = "Payment")]
    public int? PaymentId { get; set; }

    [Display(Name = "Refund Date")]
    public DateOnly? RefundDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Refund amount must be greater than zero.")]
    public decimal RefundAmount { get; set; }

    [StringLength(500)]
    [Display(Name = "Reason")]
    public string? RefundReason { get; set; }

    [StringLength(30)]
    public string? RefundStatus { get; set; } = "Completed";

    [StringLength(30)]
    [Display(Name = "Method")]
    public string? PaymentMethod { get; set; }

    public IReadOnlyList<Booking> BookingOptions { get; set; } = Array.Empty<Booking>();
    public IReadOnlyList<Payment> PaymentOptions { get; set; } = Array.Empty<Payment>();
}

public class ExpenseListViewModel
{
    public IReadOnlyList<Expense> Expenses { get; set; } = Array.Empty<Expense>();
}

public class ExpenseInputModel
{
    public int ExpenseId { get; set; }

    [Display(Name = "Booking")]
    public int? BookingId { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [Required]
    [Display(Name = "Expense Date")]
    public DateOnly ExpenseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(100)]
    [Display(Name = "Category")]
    public string? ExpenseCategory { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [StringLength(30)]
    [Display(Name = "Method")]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    [Display(Name = "Reference No")]
    public string? ReferenceNo { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public IReadOnlyList<Booking> BookingOptions { get; set; } = Array.Empty<Booking>();
    public IReadOnlyList<Supplier> SupplierOptions { get; set; } = Array.Empty<Supplier>();
}

public class SupplierPaymentListViewModel
{
    public IReadOnlyList<SupplierPayment> Payments { get; set; } = Array.Empty<SupplierPayment>();
}

public class SupplierPaymentInputModel
{
    public int SupplierPaymentId { get; set; }

    [Required]
    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Display(Name = "Booking")]
    public int? BookingId { get; set; }

    [Required]
    [Display(Name = "Payment Date")]
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(30)]
    [Display(Name = "Method")]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    [Display(Name = "Reference No")]
    public string? ReferenceNo { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public IReadOnlyList<Supplier> SupplierOptions { get; set; } = Array.Empty<Supplier>();
    public IReadOnlyList<Booking> BookingOptions { get; set; } = Array.Empty<Booking>();
}

public class LedgerFilterModel
{
  public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}

public class CustomerLedgerRow
{
    public DateTime? TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

public class SupplierLedgerRow
{
    public DateOnly TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

public class ProfitLossSummaryViewModel
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public decimal TotalCollections { get; set; }
    public decimal TotalRefunds { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalSupplierPayments { get; set; }
    public decimal GrossProfit { get; set; }
}