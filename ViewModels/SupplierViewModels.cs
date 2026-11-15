using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class SupplierListViewModel
{
    public IReadOnlyList<Supplier> Suppliers { get; set; } = Array.Empty<Supplier>();
    public string? TypeFilter { get; set; }
    public bool IncludeInactive { get; set; }
}

public class SupplierInputModel
{
    public int SupplierId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Supplier Name")]
    public string SupplierName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Display(Name = "Supplier Type")]
    public string SupplierType { get; set; } = "General";

    [StringLength(150)]
    [Display(Name = "Contact Person")]
    public string? ContactPerson { get; set; }

    [StringLength(30)]
    [Display(Name = "Mobile")]
    public string? MobileNo { get; set; }

    [StringLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Country")]
    public int? CountryId { get; set; }

    [Display(Name = "City")]
    public int? CityId { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public IReadOnlyList<Country> CountryOptions { get; set; } = Array.Empty<Country>();
    public IReadOnlyList<City> CityOptions { get; set; } = Array.Empty<City>();
}

public class HotelListViewModel
{
    public IReadOnlyList<Hotel> Hotels { get; set; } = Array.Empty<Hotel>();
    public bool IncludeInactive { get; set; }
}

public class HotelInputModel
{
    public int HotelId { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [Required]
    [Display(Name = "City")]
    public int CityId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Hotel Name")]
    public string HotelName { get; set; } = string.Empty;

    [Display(Name = "Star Rating")]
    public int? StarRating { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Display(Name = "Distance from Haram (KM)")]
    public decimal? DistanceFromHaramKm { get; set; }

    [Display(Name = "Distance from Nabawi (KM)")]
    public decimal? DistanceFromNabawiKm { get; set; }

    [StringLength(30)]
    [Display(Name = "Contact No")]
    public string? ContactNo { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public IReadOnlyList<Supplier> SupplierOptions { get; set; } = Array.Empty<Supplier>();
    public IReadOnlyList<City> CityOptions { get; set; } = Array.Empty<City>();
}

public class TransportProviderListViewModel
{
    public IReadOnlyList<TransportProvider> Providers { get; set; } = Array.Empty<TransportProvider>();
    public bool IncludeInactive { get; set; }
}

public class TransportProviderInputModel
{
    public int TransportProviderId { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Provider Name")]
    public string ProviderName { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Vehicle Type")]
    public string? VehicleType { get; set; }

    [Display(Name = "Capacity")]
    public int? VehicleCapacity { get; set; }

    [StringLength(30)]
    [Display(Name = "Contact No")]
    public string? ContactNo { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public IReadOnlyList<Supplier> SupplierOptions { get; set; } = Array.Empty<Supplier>();
}