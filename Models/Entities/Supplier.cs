using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Supplier
{
    [Key]
    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [StringLength(150)]
    public string SupplierName { get; set; } = null!;

    [StringLength(50)]
    public string SupplierType { get; set; } = null!;

    [StringLength(150)]
    public string? ContactPerson { get; set; }

    [StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [Column("CountryID")]
    public int? CountryId { get; set; }

    [Column("CityID")]
    public int? CityId { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Suppliers")]
    public virtual City? City { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Suppliers")]
    public virtual Country? Country { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<DepartureFlight> DepartureFlights { get; set; } = new List<DepartureFlight>();

    [InverseProperty("Supplier")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [InverseProperty("Supplier")]
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    [InverseProperty("Supplier")]
    public virtual ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();

    [InverseProperty("Supplier")]
    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();

    [InverseProperty("Supplier")]
    public virtual ICollection<TransportProvider> TransportProviders { get; set; } = new List<TransportProvider>();

    [InverseProperty("Supplier")]
    public virtual ICollection<VisaApplication> VisaApplications { get; set; } = new List<VisaApplication>();
}
