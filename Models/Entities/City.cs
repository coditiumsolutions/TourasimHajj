using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class City
{
    [Key]
    [Column("CityID")]
    public int CityId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(100)]
    public string CityName { get; set; } = null!;

    public bool? IsActive { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    public virtual Country Country { get; set; } = null!;

    [InverseProperty("City")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("FromCity")]
    public virtual ICollection<DepartureFlight> DepartureFlightFromCities { get; set; } = new List<DepartureFlight>();

    [InverseProperty("ToCity")]
    public virtual ICollection<DepartureFlight> DepartureFlightToCities { get; set; } = new List<DepartureFlight>();

    [InverseProperty("City")]
    public virtual ICollection<Destination> Destinations { get; set; } = new List<Destination>();

    [InverseProperty("City")]
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    [InverseProperty("City")]
    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
