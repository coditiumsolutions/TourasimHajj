using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class PackageListViewModel
{
    public IReadOnlyList<TravelPackage> Packages { get; set; } = Array.Empty<TravelPackage>();
    public string? PackageTypeFilter { get; set; }
    public static readonly string[] PackageTypes = { "Hajj", "Umrah", "Tour", "Custom" };
}

public class PackageBasicInputModel
{
    public int PackageId { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Package Code")]
    public string PackageCode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Package Name")]
    public string PackageName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    [Display(Name = "Package Type")]
    public string PackageType { get; set; } = "Umrah";

    public string? Description { get; set; }

    [Display(Name = "Duration (Days)")]
    public int? DurationDays { get; set; }

    [Display(Name = "Duration (Nights)")]
    public int? DurationNights { get; set; }

    [Display(Name = "Adult Price")]
    public decimal? AdultPrice { get; set; }

    [Display(Name = "Child Price")]
    public decimal? ChildPrice { get; set; }

    [Display(Name = "Infant Price")]
    public decimal? InfantPrice { get; set; }

    [StringLength(10)]
    [Display(Name = "Currency")]
    public string? CurrencyCode { get; set; } = "PKR";

    [Display(Name = "Minimum Persons")]
    public int? MinimumPersons { get; set; }

    [Display(Name = "Maximum Persons")]
    public int? MaximumPersons { get; set; }

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class PackageEditViewModel
{
    public PackageBasicInputModel Basic { get; set; } = new();
    public string ActiveTab { get; set; } = "basic";
    public IReadOnlyList<PackageDestinationInputModel> Destinations { get; set; } = Array.Empty<PackageDestinationInputModel>();
    public IReadOnlyList<PackageItineraryInputModel> Itineraries { get; set; } = Array.Empty<PackageItineraryInputModel>();
    public IReadOnlyList<PackageServiceInputModel> Services { get; set; } = Array.Empty<PackageServiceInputModel>();
    public IReadOnlyList<PackageDepartureInputModel> Departures { get; set; } = Array.Empty<PackageDepartureInputModel>();
    public IReadOnlyList<Destination> DestinationOptions { get; set; } = Array.Empty<Destination>();
    public IReadOnlyList<Service> ServiceOptions { get; set; } = Array.Empty<Service>();
}

public class PackageDestinationInputModel
{
    public int PackageDestinationId { get; set; }
    public int DestinationId { get; set; }
    public string? DestinationName { get; set; }
    public int SequenceNo { get; set; } = 1;
    public int NumberOfDays { get; set; } = 1;
}

public class PackageItineraryInputModel
{
    public int ItineraryId { get; set; }
    public int DayNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? DestinationId { get; set; }
    public string? DestinationName { get; set; }
}

public class PackageServiceInputModel
{
    public int PackageServiceId { get; set; }
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public bool IsIncluded { get; set; } = true;
    public decimal AdditionalPrice { get; set; }
    public string? Remarks { get; set; }
}

public class PackageDepartureInputModel
{
    public int DepartureId { get; set; }

    [Required]
    [Display(Name = "Departure Date")]
    [DataType(DataType.Date)]
    public DateOnly DepartureDate { get; set; }

    [Required]
    [Display(Name = "Return Date")]
    [DataType(DataType.Date)]
    public DateOnly ReturnDate { get; set; }

    [Display(Name = "Total Seats")]
    public int TotalSeats { get; set; }

    [Display(Name = "Available Seats")]
    public int AvailableSeats { get; set; }

    public decimal? AdultPrice { get; set; }
    public decimal? ChildPrice { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; } = "PKR";

    [StringLength(30)]
    [Display(Name = "Status")]
    public string? DepartureStatus { get; set; } = "Open";
}