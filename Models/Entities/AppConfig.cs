using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismHajj.Models.Entities;

[Table("AppConfigs")]
public class AppConfig
{
    [Key]
    public int AppConfigId { get; set; }

    [Required, StringLength(100)]
    public string ConfigKey { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string ConfigValue { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
