using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class PackageDestination
{
    [Key]
    [Column("PackageDestinationID")]
    public int PackageDestinationId { get; set; }

    [Column("PackageID")]
    public int PackageId { get; set; }

    [Column("DestinationID")]
    public int DestinationId { get; set; }

    public int? SequenceNo { get; set; }

    public int? NumberOfDays { get; set; }

    [ForeignKey("DestinationId")]
    [InverseProperty("PackageDestinations")]
    public virtual Destination Destination { get; set; } = null!;

    [ForeignKey("PackageId")]
    [InverseProperty("PackageDestinations")]
    public virtual TravelPackage Package { get; set; } = null!;
}
