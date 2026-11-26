using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class FlightSearchRequest
{
    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "From")]
    public string Origin { get; set; } = "BAH";

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "To")]
    public string Destination { get; set; } = "JED";

    [Required, DataType(DataType.Date)]
    public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(21);

    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; } = DateTime.Today.AddDays(28);

    [Range(1, 9)]
    public int Adults { get; set; } = 1;

    [Range(0, 8)]
    public int Children { get; set; }

    public bool OneWay { get; set; }
}

public class FlightOfferItem
{
    public string OfferId { get; set; } = string.Empty;
    public string CarrierCode { get; set; } = string.Empty;
    public string CarrierName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureAt { get; set; }
    public DateTime ArrivalAt { get; set; }
    public string Duration { get; set; } = string.Empty;
    public int Stops { get; set; }
    public string Cabin { get; set; } = "ECONOMY";
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "BHD";
    public bool IsRoundTrip { get; set; }
    public string? ReturnFlightNumber { get; set; }
    public DateTime? ReturnDepartureAt { get; set; }
    public DateTime? ReturnArrivalAt { get; set; }
    public string? Summary { get; set; }
}

public class FlightBookRequest
{
    public string OfferId { get; set; } = string.Empty;
    public string TravelerFirstName { get; set; } = string.Empty;
    public string TravelerLastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string ClientReference { get; set; } = string.Empty;
}

public class FlightProviderBookingResult
{
    public bool Success { get; set; }
    public string? SupplierBookingReference { get; set; }
    public string? TicketNumber { get; set; }
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Currency { get; set; }
    public string? RawJson { get; set; }
}

public class FlightSearchPageViewModel
{
    public FlightSearchRequest Search { get; set; } = new();
    public IReadOnlyList<FlightOfferItem> Results { get; set; } = Array.Empty<FlightOfferItem>();
    public bool ApiConfigured { get; set; }
    public bool IsDemoMode { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Searched { get; set; }
    public string PaymentProvider { get; set; } = "Manual";
}

public class FlightBookPageViewModel
{
    [Required]
    public string OfferId { get; set; } = string.Empty;

    public string CarrierName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureAt { get; set; }
    public DateTime ArrivalAt { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "BHD";
    public string? Summary { get; set; }
    public bool IsRoundTrip { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "First name")]
    public string TravelerFirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "Last name")]
    public string TravelerLastName { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "Manual";

    public IReadOnlyList<string> AvailablePaymentMethods { get; set; } = Array.Empty<string>();
    public string? ErrorMessage { get; set; }
}

public class FlightReservationListItem
{
    public int FlightReservationId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? SupplierBookingReference { get; set; }
    public string? TicketNumber { get; set; }
    public string Route { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? SellingAmount { get; set; }
    public string? Currency { get; set; }
    public string HolderName { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime CreatedDate { get; set; }
}
