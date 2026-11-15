using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Destination
{
    [Key]
    [Column("DestinationID")]
    public int DestinationId { get; set; }

    [Column("CityID")]
    public int CityId { get; set; }

    [StringLength(150)]
    public string DestinationName { get; set; } = null!;

    public string? Description { get; set; }

    [Column("ImageURL")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Destinations")]
    public virtual City City { get; set; } = null!;

    [InverseProperty("Destination")]
    public virtual ICollection<PackageDestination> PackageDestinations { get; set; } = new List<PackageDestination>();

    [InverseProperty("Destination")]
    public virtual ICollection<PackageItinerary> PackageItineraries { get; set; } = new List<PackageItinerary>();
}
