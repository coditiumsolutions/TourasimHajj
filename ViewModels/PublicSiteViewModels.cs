using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class PublicHomeViewModel
{
    public PackageSearchViewModel Search { get; set; } = new();
    public List<PublicPackageCardViewModel> FeaturedPackages { get; set; } = [];
    public List<PublicPackageCardViewModel> HajjPackages { get; set; } = [];
    public List<PublicPackageCardViewModel> UmrahPackages { get; set; } = [];
    public List<PublicDestinationCardViewModel> Destinations { get; set; } = [];
    public ContactFormViewModel Contact { get; set; } = new();
}

public class PackageSearchViewModel
{
    public string? PackageType { get; set; }
    public int? DestinationId { get; set; }
    public DateOnly? DepartureDate { get; set; }
    public int Travelers { get; set; } = 1;
    public List<Destination> DestinationOptions { get; set; } = [];
}

public class PublicPackageCardViewModel
{
    public int PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DestinationName { get; set; }
    public int? DurationDays { get; set; }
    public int? DurationNights { get; set; }
    public decimal? AdultPrice { get; set; }
    public decimal? ChildPrice { get; set; }
    public decimal? InfantPrice { get; set; }
    public string? CurrencyCode { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateOnly? NextDepartureDate { get; set; }
    public string? AccommodationSummary { get; set; }
}

public class PublicDestinationCardViewModel
{
    public int DestinationId { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? CityName { get; set; }
}

public class PublicPackageDetailsViewModel
{
    public TravelPackage Package { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public string? DestinationSummary { get; set; }
    public List<PackageItinerary> Itineraries { get; set; } = [];
    public List<Models.Entities.PackageService> IncludedServices { get; set; } = [];
    public List<Models.Entities.PackageService> ExcludedServices { get; set; } = [];
    public List<PackageDeparture> Departures { get; set; } = [];
    public List<Hotel> Hotels { get; set; } = [];
}

public class ContactFormViewModel
{
    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Subject { get; set; }

    [Required, StringLength(4000)]
    public string Message { get; set; } = string.Empty;
}

public class CustomerLoginViewModel
{
    [Required]
    [Display(Name = "Email or Mobile")]
    public string EmailOrMobile { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class CustomerRegisterViewModel
{
    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Father Name")]
    public string? FatherName { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Mobile Number")]
    public string MobileNo { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "WhatsApp Number")]
    public string? WhatsAppNo { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "CNIC")]
    public string? Cnic { get; set; }

    [Required, DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AdminLoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class CustomerPortalDashboardViewModel
{
    public string CustomerName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public int UpcomingTrips { get; set; }
    public int PendingPayments { get; set; }
    public int CompletedTrips { get; set; }
    public List<Booking> RecentBookings { get; set; } = [];
    public List<PackageDeparture> UpcomingDepartures { get; set; } = [];
    public List<Payment> RecentPayments { get; set; } = [];
    public List<TravelerDocument> PendingDocuments { get; set; } = [];
}

public class CustomerProfileViewModel
{
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(150)]
    public string? FatherName { get; set; }

    [StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(30)]
    public string? WhatsAppNo { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}

public class CustomerBookingRequestViewModel
{
    public int PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public int? DepartureId { get; set; }
    public int TotalAdults { get; set; } = 1;
    public int TotalChildren { get; set; }
    public int TotalInfants { get; set; }
    public List<int> TravelerIds { get; set; } = [];
    public List<Traveler> TravelerOptions { get; set; } = [];
    public List<PackageDeparture> DepartureOptions { get; set; } = [];
}
