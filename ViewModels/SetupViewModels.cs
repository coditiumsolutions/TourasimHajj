using System.ComponentModel.DataAnnotations;

namespace TourismHajj.ViewModels;

public class AppConfigInputModel
{
    public int AppConfigId { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Config Key")]
    public string ConfigKey { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    [Display(Name = "Config Value")]
    public string ConfigValue { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
