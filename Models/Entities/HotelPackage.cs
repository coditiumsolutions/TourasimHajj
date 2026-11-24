using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismHajj.Models.Entities;

[Table("HotelPackages")]
public class HotelPackage
{
    [Key]
    public int HotelPackageId { get; set; }

    [Required, StringLength(150)]
    public string HotelName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? CityName { get; set; }

    public int? StarRating { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DistanceFromHaramKm { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DistanceFromNabawiKm { get; set; }

    [StringLength(30)]
    public string? ContactNo { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PricePerNight { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; } = "PKR";

    public int? DurationNights { get; set; }

    public int SortOrder { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }
}
