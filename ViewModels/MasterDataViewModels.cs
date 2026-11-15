using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class CountryViewModel
{
    public int CountryId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Country Name")]
    public string CountryName { get; set; } = string.Empty;

    [StringLength(10)]
    [Display(Name = "Country Code")]
    public string? CountryCode { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class CityViewModel
{
    public int CityId { get; set; }

    [Required]
    [Display(Name = "Country")]
    public int CountryId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "City Name")]
    public string CityName { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public string? CountryName { get; set; }
}

public class DestinationViewModel
{
    public int DestinationId { get; set; }

    [Required]
    [Display(Name = "City")]
    public int CityId { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Destination Name")]
    public string DestinationName { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [StringLength(500)]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public string? CityName { get; set; }
}

public class ServiceViewModel
{
    public int ServiceId { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Service Name")]
    public string ServiceName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Category")]
    public string? ServiceCategory { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class EmployeeViewModel
{
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Employee Name")]
    public string EmployeeName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Designation { get; set; }

    [StringLength(30)]
    [Display(Name = "Mobile No")]
    public string? MobileNo { get; set; }

    [StringLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Joining Date")]
    [DataType(DataType.Date)]
    public DateOnly? JoiningDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Salary { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}