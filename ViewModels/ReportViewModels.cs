using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class ReportIndexViewModel
{
    public IReadOnlyList<ReportMenuItem> Reports { get; set; } = Array.Empty<ReportMenuItem>();
}

public class ReportMenuItem
{
    public string Title { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ReportFilterViewModel
{
    [Display(Name = "From Date")]
    public DateOnly? FromDate { get; set; }

    [Display(Name = "To Date")]
    public DateOnly? ToDate { get; set; }

    [Display(Name = "Status")]
    public string? Status { get; set; }

    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [Display(Name = "Package Type")]
    public string? PackageType { get; set; }

    [Display(Name = "Booking No")]
    public string? BookingNo { get; set; }

    [Display(Name = "Passport expiring within (days)")]
    public int PassportExpiryDays { get; set; } = 90;

    public string ReportTitle { get; set; } = string.Empty;
    public string ReportAction { get; set; } = string.Empty;
}

public class BookingRegisterRow
{
    public string BookingNo { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? PackageName { get; set; }
    public DateTime? BookingDate { get; set; }
    public string? BookingStatus { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public string? PaymentStatus { get; set; }
}

public class CollectionRow
{
    public DateOnly CollectionDate { get; set; }
    public string? PaymentMethod { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class MonthlyCollectionRow
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class OutstandingPaymentRow
{
    public string BookingNo { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? MobileNo { get; set; }
    public DateTime? BookingDate { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public string? PaymentStatus { get; set; }
}

public class PackageSalesRow
{
    public string? PackageCode { get; set; }
    public string? PackageName { get; set; }
    public string? PackageType { get; set; }
    public int BookingCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class VisaStatusReportRow
{
    public string? TravelerName { get; set; }
    public string? BookingNo { get; set; }
    public string? VisaType { get; set; }
    public string? VisaStatus { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public DateOnly? VisaExpiryDate { get; set; }
    public string? SupplierName { get; set; }
}

public class PassportExpiryRow
{
    public string? TravelerName { get; set; }
    public string? CustomerName { get; set; }
    public string? PassportNo { get; set; }
    public DateOnly? PassportExpiryDate { get; set; }
    public int DaysToExpiry { get; set; }
}

public class HotelRoomAllocationRow
{
    public string? HotelName { get; set; }
    public string? CityName { get; set; }
    public string? PilgrimName { get; set; }
    public string? BookingNo { get; set; }
    public string? RoomNumber { get; set; }
    public string? RoomType { get; set; }
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
}

public class SupplierPaymentReportRow
{
    public DateOnly PaymentDate { get; set; }
    public string? SupplierName { get; set; }
    public string? BookingNo { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNo { get; set; }
    public decimal Amount { get; set; }
}

public class ExpenseRegisterRow
{
    public DateOnly ExpenseDate { get; set; }
    public string? ExpenseCategory { get; set; }
    public string? Description { get; set; }
    public string? SupplierName { get; set; }
    public string? BookingNo { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
}

public class ReportResultViewModel<T>
{
    public ReportFilterViewModel Filter { get; set; } = new();
    public IReadOnlyList<T> Rows { get; set; } = Array.Empty<T>();
}