using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class TransportProvider
{
    [Key]
    [Column("TransportProviderID")]
    public int TransportProviderId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    [StringLength(150)]
    public string ProviderName { get; set; } = null!;

    [StringLength(100)]
    public string? VehicleType { get; set; }

    public int? VehicleCapacity { get; set; }

    [StringLength(30)]
    public string? ContactNo { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("TransportProvider")]
    public virtual ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();

    [ForeignKey("SupplierId")]
    [InverseProperty("TransportProviders")]
    public virtual Supplier? Supplier { get; set; }
}
