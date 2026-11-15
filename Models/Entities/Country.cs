using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Country
{
    [Key]
    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(100)]
    public string CountryName { get; set; } = null!;

    [StringLength(10)]
    public string? CountryCode { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Country")]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    [InverseProperty("Country")]
    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    [InverseProperty("NationalityCountry")]
    public virtual ICollection<Traveler> Travelers { get; set; } = new List<Traveler>();

    [InverseProperty("Country")]
    public virtual ICollection<VisaApplication> VisaApplications { get; set; } = new List<VisaApplication>();
}
