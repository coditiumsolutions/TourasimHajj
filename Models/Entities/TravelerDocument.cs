using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class TravelerDocument
{
    [Key]
    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [Column("TravelerID")]
    public int TravelerId { get; set; }

    [StringLength(50)]
    public string DocumentType { get; set; } = null!;

    [StringLength(100)]
    public string? DocumentNumber { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime? UploadedDate { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("TravelerId")]
    [InverseProperty("TravelerDocuments")]
    public virtual Traveler Traveler { get; set; } = null!;
}
