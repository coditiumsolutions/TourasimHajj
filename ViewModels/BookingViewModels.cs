using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class BookingListViewModel
{
    public IReadOnlyList<Booking> Bookings { get; set; } = Array.Empty<Booking>();
}

public class BookingWizardViewModel
{
  public int Step { get; set; } = 1;

    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Required]
    [Display(Name = "Package")]
    public int PackageId { get; set; }

    [Display(Name = "Departure")]
    public int? DepartureId { get; set; }

    [StringLength(30)]
    [Display(Name = "Booking Type")]
    public string? BookingType { get; set; }

    public int TotalAdults { get; set; } = 1;
    public int TotalChildren { get; set; }
    public int TotalInfants { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public List<BookingTravelerLineViewModel> TravelerLines { get; set; } = new();

    public bool IncludeInitialPayment { get; set; }
    public decimal? InitialPaymentAmount { get; set; }
    public string? InitialPaymentMethod { get; set; }
    public string? InitialPaymentReference { get; set; }

    public decimal CalculatedTotal { get; set; }
    public decimal CalculatedNet { get; set; }

    public IReadOnlyList<Customer> CustomerOptions { get; set; } = Array.Empty<Customer>();
    public IReadOnlyList<TravelPackage> PackageOptions { get; set; } = Array.Empty<TravelPackage>();
    public IReadOnlyList<PackageDeparture> DepartureOptions { get; set; } = Array.Empty<PackageDeparture>();
    public IReadOnlyList<Traveler> CustomerTravelers { get; set; } = Array.Empty<Traveler>();
}

public class BookingTravelerLineViewModel
{
    public int TravelerId { get; set; }
    public string? TravelerName { get; set; }
    public bool Selected { get; set; }
    public string TravelerCategory { get; set; } = "Adult";
    public decimal PackagePrice { get; set; }
    public decimal DiscountAmount { get; set; }
}

public class BookingDetailsViewModel
{
    public Booking Booking { get; set; } = null!;
    public IReadOnlyList<BookingTraveler> Travelers { get; set; } = Array.Empty<BookingTraveler>();
    public IReadOnlyList<Payment> Payments { get; set; } = Array.Empty<Payment>();
}

public class BookingPrintViewModel
{
    public Booking Booking { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public TravelPackage Package { get; set; } = null!;
    public PackageDeparture? Departure { get; set; }
    public IReadOnlyList<BookingTraveler> Travelers { get; set; } = Array.Empty<BookingTraveler>();
}