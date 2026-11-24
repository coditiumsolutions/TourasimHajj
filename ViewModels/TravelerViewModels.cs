using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class TravelerListViewModel
{
    public IReadOnlyList<Traveler> Travelers { get; set; } = Array.Empty<Traveler>();
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

public class TravelerInputModel
{
    public int TravelerId { get; set; }

    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Father Name")]
    public string? FatherName { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Nationality")]
    public int? NationalityCountryId { get; set; }

    [StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(50)]
    [Display(Name = "Passport No")]
    public string? PassportNo { get; set; }

    [Display(Name = "Passport Issue")]
    [DataType(DataType.Date)]
    public DateOnly? PassportIssueDate { get; set; }

    [Display(Name = "Passport Expiry")]
    [DataType(DataType.Date)]
    public DateOnly? PassportExpiryDate { get; set; }

    [StringLength(30)]
    [Display(Name = "Mobile No")]
    public string? MobileNo { get; set; }

    [StringLength(50)]
    public string? Relationship { get; set; }
}

public class TravelerDetailsViewModel
{
    public Traveler Traveler { get; set; } = null!;
    public IReadOnlyList<TravelerDocument> Documents { get; set; } = Array.Empty<TravelerDocument>();
    public bool PassportExpiringSoon { get; set; }
    public bool PassportExpired { get; set; }
}

public class PassportAlertViewModel
{
    public IReadOnlyList<Traveler> ExpiringTravelers { get; set; } = Array.Empty<Traveler>();
    public int WithinDays { get; set; }
}

public class TravelerDocumentInputModel
{
    [Required]
    [Display(Name = "Traveler")]
    public int TravelerId { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Document Type")]
    public string DocumentType { get; set; } = "Passport";

    [StringLength(100)]
    [Display(Name = "Document Number")]
    public string? DocumentNumber { get; set; }

    [Display(Name = "Issue Date")]
    [DataType(DataType.Date)]
    public DateOnly? IssueDate { get; set; }

    [Display(Name = "Expiry Date")]
    [DataType(DataType.Date)]
    public DateOnly? ExpiryDate { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }
}

public class TravelerDocumentsPageViewModel
{
    public int? SelectedTravelerId { get; set; }
    public string? Search { get; set; }
    public IReadOnlyList<TravelerDocumentListItem> Travelers { get; set; } = Array.Empty<TravelerDocumentListItem>();
    public Traveler? SelectedTraveler { get; set; }
    public IReadOnlyList<TravelerDocument> Documents { get; set; } = Array.Empty<TravelerDocument>();
    public TravelerDocumentInputModel Upload { get; set; } = new();
}

public class TravelerDocumentListItem
{
    public int TravelerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? PassportNo { get; set; }
    public int DocumentCount { get; set; }
}