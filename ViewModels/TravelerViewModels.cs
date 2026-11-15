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