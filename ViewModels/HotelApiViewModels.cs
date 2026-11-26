using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class HotelSearchRequest
{
    [Required]
    public string Destination { get; set; } = "Makkah"; // Makkah | Madinah

    [Required, DataType(DataType.Date)]
    public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(14);

    [Required, DataType(DataType.Date)]
    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(17);

    [Range(1, 9)]
    public int Rooms { get; set; } = 1;

    [Range(1, 12)]
    public int Adults { get; set; } = 2;

    [Range(0, 8)]
    public int Children { get; set; }

    public int? MinStars { get; set; }
}

public class HotelSearchResultItem
{
    public string HotelCode { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string? DestinationName { get; set; }
    public string? ZoneName { get; set; }
    public int? CategoryCode { get; set; }
    public string? CategoryName { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal MinRate { get; set; }
    public string Currency { get; set; } = "USD";
    public List<HotelRoomOffer> Rooms { get; set; } = new();
}

public class HotelRoomOffer
{
    public string RateKey { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string? BoardCode { get; set; }
    public string? BoardName { get; set; }
    public decimal Net { get; set; }
    public decimal SellingRate { get; set; }
    public string Currency { get; set; } = "USD";
    public bool RateCommentsRequired { get; set; }
    public string? CancellationPolicySummary { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
}

public class HotelRateCheckResult
{
    public string RateKey { get; set; } = string.Empty;
    public string HotelCode { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string? BoardName { get; set; }
    public decimal Net { get; set; }
    public decimal SellingRate { get; set; }
    public string Currency { get; set; } = "USD";
    public string? CancellationPolicySummary { get; set; }
    public string? RateComments { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}

public class HotelBookRequest
{
    public string RateKey { get; set; } = string.Empty;
    public string HolderFirstName { get; set; } = string.Empty;
    public string HolderLastName { get; set; } = string.Empty;
    public string? HolderEmail { get; set; }
    public string? HolderPhone { get; set; }
    public string ClientReference { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public List<HotelBookPax> Guests { get; set; } = new();
}

public class HotelBookPax
{
    public string Type { get; set; } = "AD"; // AD | CH
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
}

public class HotelProviderBookingResult
{
    public bool Success { get; set; }
    public string? SupplierBookingReference { get; set; }
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? TotalNet { get; set; }
    public decimal? TotalSelling { get; set; }
    public string? Currency { get; set; }
    public string? HotelCode { get; set; }
    public string? HotelName { get; set; }
    public string? RawJson { get; set; }
}

public class HotelSearchPageViewModel
{
    public HotelSearchRequest Search { get; set; } = new();
    public IReadOnlyList<HotelSearchResultItem> LiveResults { get; set; } = Array.Empty<HotelSearchResultItem>();
    public IReadOnlyList<PublicHotelCardViewModel> ManualPackages { get; set; } = Array.Empty<PublicHotelCardViewModel>();
    /// <summary>True when search/results are available (live Hotelbeds or demo fallback).</summary>
    public bool ApiConfigured { get; set; }
    /// <summary>True when showing sample inventory because Hotelbeds keys are not set.</summary>
    public bool IsDemoMode { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Searched { get; set; }
}

public class HotelDetailPageViewModel
{
    public HotelSearchRequest Search { get; set; } = new();
    public HotelSearchResultItem Hotel { get; set; } = new();
}

public class HotelBookPageViewModel
{
    [Required]
    public string RateKey { get; set; } = string.Empty;

    public string HotelCode { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string Destination { get; set; } = "Makkah";
    public string RoomName { get; set; } = string.Empty;
    public string? BoardName { get; set; }
    public decimal SellingRate { get; set; }
    public string Currency { get; set; } = "USD";
    public string? CancellationPolicySummary { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckIn { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckOut { get; set; }

    [Required, StringLength(80)]
    public string HolderFirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string HolderLastName { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? HolderEmail { get; set; }

    [StringLength(30)]
    public string? HolderPhone { get; set; }

    [StringLength(80)]
    public string GuestFirstName { get; set; } = string.Empty;

    [StringLength(80)]
    public string GuestLastName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

public class HotelReservationListItem
{
    public int HotelReservationId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? SupplierBookingReference { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? SellingAmount { get; set; }
    public string? Currency { get; set; }
    public string? HolderName { get; set; }
    public DateTime CreatedDate { get; set; }
}
