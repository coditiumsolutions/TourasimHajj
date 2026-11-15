using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class VisaApplication
{
    [Key]
    [Column("VisaApplicationID")]
    public int VisaApplicationId { get; set; }

    [Column("BookingTravelerID")]
    public int BookingTravelerId { get; set; }

    [StringLength(30)]
    public string VisaType { get; set; } = null!;

    [Column("CountryID")]
    public int CountryId { get; set; }

    public DateOnly? ApplicationDate { get; set; }

    [StringLength(100)]
    public string? ApplicationNumber { get; set; }

    [StringLength(100)]
    public string? VisaNumber { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public DateOnly? ApprovalDate { get; set; }

    public DateOnly? VisaIssueDate { get; set; }

    public DateOnly? VisaExpiryDate { get; set; }

    [StringLength(30)]
    public string? VisaStatus { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingTravelerId")]
    [InverseProperty("VisaApplications")]
    public virtual BookingTraveler BookingTraveler { get; set; } = null!;

    [ForeignKey("CountryId")]
    [InverseProperty("VisaApplications")]
    public virtual Country Country { get; set; } = null!;

    [ForeignKey("SupplierId")]
    [InverseProperty("VisaApplications")]
    public virtual Supplier? Supplier { get; set; }
}
