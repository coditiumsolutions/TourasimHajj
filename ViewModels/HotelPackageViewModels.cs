using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class HotelPackageInputModel
{
    public int HotelPackageId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Hotel Name")]
    public string HotelName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? CityName { get; set; }

    [Display(Name = "Star Rating")]
    [Range(1, 7)]
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

    [StringLength(500)]
    [Display(Name = "Cover Image URL")]
    public string? CoverImageUrl { get; set; }

    [Display(Name = "Price Per Night")]
    public decimal? PricePerNight { get; set; }

    [StringLength(10)]
    [Display(Name = "Currency")]
    public string? CurrencyCode { get; set; } = "PKR";

    [Display(Name = "Nights")]
    public int? DurationNights { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
