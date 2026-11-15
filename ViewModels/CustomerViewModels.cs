using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class CustomerListViewModel
{
    public IReadOnlyList<Customer> Customers { get; set; } = Array.Empty<Customer>();
    public string? SearchTerm { get; set; }
}

public class CustomerInputModel
{
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Father Name")]
    public string? FatherName { get; set; }

    [StringLength(30)]
    [Display(Name = "Mobile No")]
    public string? MobileNo { get; set; }

    [StringLength(30)]
    [Display(Name = "WhatsApp No")]
    public string? WhatsAppNo { get; set; }

    [StringLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Display(Name = "City")]
    public int? CityId { get; set; }

    [StringLength(30)]
    [Display(Name = "Customer Type")]
    public string? CustomerType { get; set; } = "Individual";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class CustomerDetailsViewModel
{
    public Customer Customer { get; set; } = null!;
    public IReadOnlyList<Traveler> Travelers { get; set; } = Array.Empty<Traveler>();
    public IReadOnlyList<BookingSummaryRow> Bookings { get; set; } = Array.Empty<BookingSummaryRow>();
    public IReadOnlyList<PaymentRow> Payments { get; set; } = Array.Empty<PaymentRow>();
    public decimal TotalOutstanding { get; set; }
}

public class BookingSummaryRow
{
    public int BookingId { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public string? PackageName { get; set; }
    public DateTime? BookingDate { get; set; }
    public string? BookingStatus { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
}

public class PaymentRow
{
    public int PaymentId { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal Amount { get; set; }
}