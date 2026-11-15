using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class DepartureFlight
{
    [Key]
    [Column("DepartureFlightID")]
    public int DepartureFlightId { get; set; }

    [Column("DepartureID")]
    public int DepartureId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    [StringLength(150)]
    public string? AirlineName { get; set; }

    [StringLength(50)]
    public string? FlightNumber { get; set; }

    [Column("FromCityID")]
    public int? FromCityId { get; set; }

    [Column("ToCityID")]
    public int? ToCityId { get; set; }

    public DateTime? DepartureDateTime { get; set; }

    public DateTime? ArrivalDateTime { get; set; }

    [StringLength(30)]
    public string? FlightType { get; set; }

    [ForeignKey("DepartureId")]
    [InverseProperty("DepartureFlights")]
    public virtual PackageDeparture Departure { get; set; } = null!;

    [ForeignKey("FromCityId")]
    [InverseProperty("DepartureFlightFromCities")]
    public virtual City? FromCity { get; set; }

    [ForeignKey("SupplierId")]
    [InverseProperty("DepartureFlights")]
    public virtual Supplier? Supplier { get; set; }

    [ForeignKey("ToCityId")]
    [InverseProperty("DepartureFlightToCities")]
    public virtual City? ToCity { get; set; }
}
