using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismHajj.Models.Entities;

[Table("ContactInquiries")]
public class ContactInquiry
{
    [Key]
    public int InquiryId { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Subject { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }
}
