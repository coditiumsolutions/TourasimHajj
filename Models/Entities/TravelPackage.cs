using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

[Index("PackageCode", Name = "UQ__TravelPa__94185429D6CB0E60", IsUnique = true)]
public partial class TravelPackage
{
    [Key]
    [Column("PackageID")]
    public int PackageId { get; set; }

    [StringLength(30)]
    public string PackageCode { get; set; } = null!;

    [StringLength(200)]
    public string PackageName { get; set; } = null!;

    [StringLength(30)]
    public string PackageType { get; set; } = null!;

    public string? Description { get; set; }

    public int? DurationDays { get; set; }

    public int? DurationNights { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AdultPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ChildPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? InfantPrice { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; }

    public int? MinimumPersons { get; set; }

    public int? MaximumPersons { get; set; }

    public bool? IsFeatured { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    [InverseProperty("Package")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Package")]
    public virtual ICollection<PackageDeparture> PackageDepartures { get; set; } = new List<PackageDeparture>();

    [InverseProperty("Package")]
    public virtual ICollection<PackageDestination> PackageDestinations { get; set; } = new List<PackageDestination>();

    [InverseProperty("Package")]
    public virtual ICollection<PackageItinerary> PackageItineraries { get; set; } = new List<PackageItinerary>();

    [InverseProperty("Package")]
    public virtual ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();
}
