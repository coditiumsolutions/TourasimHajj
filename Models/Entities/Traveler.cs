using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Traveler
{
    [Key]
    [Column("TravelerID")]
    public int TravelerId { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string? FatherName { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Column("NationalityCountryID")]
    public int? NationalityCountryId { get; set; }

    [Column("CNIC")]
    [StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(50)]
    public string? PassportNo { get; set; }

    public DateOnly? PassportIssueDate { get; set; }

    public DateOnly? PassportExpiryDate { get; set; }

    [StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(50)]
    public string? Relationship { get; set; }

    public DateTime? CreatedDate { get; set; }

    [InverseProperty("Traveler")]
    public virtual ICollection<BookingTraveler> BookingTravelers { get; set; } = new List<BookingTraveler>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Travelers")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("NationalityCountryId")]
    [InverseProperty("Travelers")]
    public virtual Country? NationalityCountry { get; set; }

    [InverseProperty("Traveler")]
    public virtual ICollection<TravelerDocument> TravelerDocuments { get; set; } = new List<TravelerDocument>();
}
