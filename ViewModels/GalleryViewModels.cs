using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class GalleryItemInputModel
{
    public int GalleryItemId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}

public class PublicHotelCardViewModel
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public int? StarRating { get; set; }
    public string? Address { get; set; }
    public decimal? DistanceFromHaramKm { get; set; }
    public decimal? DistanceFromNabawiKm { get; set; }
    public string? ContactNo { get; set; }
}

public class PublicGalleryCardViewModel
{
    public int GalleryItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
