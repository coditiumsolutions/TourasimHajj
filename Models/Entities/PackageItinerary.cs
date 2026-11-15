using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class PackageItinerary
{
    [Key]
    [Column("ItineraryID")]
    public int ItineraryId { get; set; }

    [Column("PackageID")]
    public int PackageId { get; set; }

    public int DayNumber { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [Column("DestinationID")]
    public int? DestinationId { get; set; }

    [ForeignKey("DestinationId")]
    [InverseProperty("PackageItineraries")]
    public virtual Destination? Destination { get; set; }

    [ForeignKey("PackageId")]
    [InverseProperty("PackageItineraries")]
    public virtual TravelPackage Package { get; set; } = null!;
}
