using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class UmrahPackageListViewModel
{
    public IReadOnlyList<TravelPackage> Packages { get; set; } = Array.Empty<TravelPackage>();
    public string? SearchTerm { get; set; }
    public bool IncludeInactive { get; set; }
}

public class VisaApplicationListViewModel
{
    public IReadOnlyList<VisaApplication> Applications { get; set; } = Array.Empty<VisaApplication>();
    public string? StatusFilter { get; set; }
    public int AlertDays { get; set; } = 14;
}

public class VisaApplicationInputModel
{
    public int VisaApplicationId { get; set; }

    [Required]
    [Display(Name = "Booking Traveler")]
    public int BookingTravelerId { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Visa Type")]
    public string VisaType { get; set; } = "Umrah";

    [Required]
    [Display(Name = "Country")]
    public int CountryId { get; set; }

    [Display(Name = "Application Date")]
    public DateOnly? ApplicationDate { get; set; }

    [StringLength(100)]
    [Display(Name = "Application No")]
    public string? ApplicationNumber { get; set; }

    [StringLength(100)]
    [Display(Name = "Visa No")]
    public string? VisaNumber { get; set; }

    [Display(Name = "Submission Date")]
    public DateOnly? SubmissionDate { get; set; }

    [Display(Name = "Approval Date")]
    public DateOnly? ApprovalDate { get; set; }

    [Display(Name = "Issue Date")]
    public DateOnly? VisaIssueDate { get; set; }

    [Display(Name = "Expiry Date")]
    public DateOnly? VisaExpiryDate { get; set; }

    [StringLength(30)]
    [Display(Name = "Status")]
    public string? VisaStatus { get; set; } = "Pending";

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public IReadOnlyList<BookingTravelerOption> TravelerOptions { get; set; } = Array.Empty<BookingTravelerOption>();
    public IReadOnlyList<Country> CountryOptions { get; set; } = Array.Empty<Country>();
    public IReadOnlyList<Supplier> SupplierOptions { get; set; } = Array.Empty<Supplier>();
}

public class VisaStatusUpdateModel
{
    public int VisaApplicationId { get; set; }

    [Required, StringLength(30)]
    public string VisaStatus { get; set; } = "Pending";

    public DateOnly? ApprovalDate { get; set; }
    public DateOnly? VisaIssueDate { get; set; }
    public DateOnly? VisaExpiryDate { get; set; }
    public string? VisaNumber { get; set; }
}

public class RoomAllocationListViewModel
{
    public IReadOnlyList<RoomAllocation> Allocations { get; set; } = Array.Empty<RoomAllocation>();
    public int? HotelId { get; set; }
}

public class RoomAllocationInputModel
{
    public int RoomAllocationId { get; set; }

    [Required]
    [Display(Name = "Booking Traveler")]
    public int BookingTravelerId { get; set; }

    [Required]
    [Display(Name = "Hotel")]
    public int HotelId { get; set; }

    [StringLength(50)]
    [Display(Name = "Room No")]
    public string? RoomNumber { get; set; }

    [StringLength(50)]
    [Display(Name = "Room Type")]
    public string? RoomType { get; set; }

    [Display(Name = "Check-In")]
    public DateOnly? CheckInDate { get; set; }

    [Display(Name = "Check-Out")]
    public DateOnly? CheckOutDate { get; set; }

    [StringLength(30)]
    [Display(Name = "Sharing")]
    public string? SharingType { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    public IReadOnlyList<BookingTravelerOption> TravelerOptions { get; set; } = Array.Empty<BookingTravelerOption>();
    public IReadOnlyList<Hotel> HotelOptions { get; set; } = Array.Empty<Hotel>();
}

public class PendingVisaAlertRow
{
    public int VisaApplicationId { get; set; }
    public string? TravelerName { get; set; }
    public string? BookingNo { get; set; }
    public string? VisaType { get; set; }
    public string? VisaStatus { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public int DaysPending { get; set; }
}

public class UmrahPilgrimRegisterRow
{
    public string? BookingNo { get; set; }
    public string? PackageName { get; set; }
    public string? PilgrimName { get; set; }
    public string? PassportNo { get; set; }
    public string? VisaStatus { get; set; }
    public string? HotelName { get; set; }
    public string? RoomNumber { get; set; }
}