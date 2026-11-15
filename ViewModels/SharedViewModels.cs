using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class PagedRequest
{
    public string? Q { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CustomerFormViewModel
{
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Father Name"), StringLength(150)]
    public string? FatherName { get; set; }

    [Display(Name = "Mobile Number"), StringLength(30)]
    public string? MobileNo { get; set; }

    [Display(Name = "WhatsApp Number"), StringLength(30)]
    public string? WhatsAppNo { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Display(Name = "CNIC"), StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Display(Name = "City")]
    public int? CityId { get; set; }

    [Display(Name = "Customer Type"), StringLength(30)]
    public string? CustomerType { get; set; }

    public bool IsActive { get; set; } = true;
}

public class TravelerFormViewModel
{
    public int TravelerId { get; set; }

    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Father Name"), StringLength(150)]
    public string? FatherName { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [Display(Name = "Date of Birth"), DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Nationality")]
    public int? NationalityCountryId { get; set; }

    [Display(Name = "CNIC"), StringLength(30)]
    public string? Cnic { get; set; }

    [Display(Name = "Passport Number"), StringLength(50)]
    public string? PassportNo { get; set; }

    [Display(Name = "Passport Issue Date"), DataType(DataType.Date)]
    public DateOnly? PassportIssueDate { get; set; }

    [Display(Name = "Passport Expiry Date"), DataType(DataType.Date)]
    public DateOnly? PassportExpiryDate { get; set; }

    [Display(Name = "Mobile Number"), StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(50)]
    public string? Relationship { get; set; }
}

public class BookingCreateViewModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PackageId { get; set; }

    public int? DepartureId { get; set; }

    public string? BookingType { get; set; }

    public int TotalAdults { get; set; } = 1;
    public int TotalChildren { get; set; }
    public int TotalInfants { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal InitialPayment { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Remarks { get; set; }

    public List<int> TravelerIds { get; set; } = new();
}

public class LedgerViewModel
{
    public string Title { get; set; } = string.Empty;
    public string PartyName { get; set; } = string.Empty;
    public List<LedgerLine> Lines { get; set; } = new();
    public decimal Balance => Lines.Sum(l => l.Debit - l.Credit);
}

public class LedgerLine
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class ProfitLossViewModel
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal SupplierPayments { get; set; }
    public decimal Refunds { get; set; }
    public decimal NetProfit => Revenue - Expenses - SupplierPayments - Refunds;
}

public class ReportResultViewModel
{
    public string Title { get; set; } = string.Empty;
    public ReportFilterViewModel Filter { get; set; } = new();
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
    public string CompanyName { get; set; } = "Tourism Hajj";
    public string CompanyAddress { get; set; } = "Travel & Pilgrimage Management";
    public string CompanyContact { get; set; } = "admin@tourismhajj.com";
}
