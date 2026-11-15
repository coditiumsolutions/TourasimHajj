using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class PackageDeparture
{
    [Key]
    [Column("DepartureID")]
    public int DepartureId { get; set; }

    [Column("PackageID")]
    public int PackageId { get; set; }

    public DateOnly DepartureDate { get; set; }

    public DateOnly ReturnDate { get; set; }

    public int? TotalSeats { get; set; }

    public int? AvailableSeats { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AdultPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ChildPrice { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; }

    [StringLength(30)]
    public string? DepartureStatus { get; set; }

    [InverseProperty("Departure")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Departure")]
    public virtual ICollection<DepartureFlight> DepartureFlights { get; set; } = new List<DepartureFlight>();

    [ForeignKey("PackageId")]
    [InverseProperty("PackageDepartures")]
    public virtual TravelPackage Package { get; set; } = null!;

    [InverseProperty("Departure")]
    public virtual ICollection<PilgrimGroup> PilgrimGroups { get; set; } = new List<PilgrimGroup>();
}
