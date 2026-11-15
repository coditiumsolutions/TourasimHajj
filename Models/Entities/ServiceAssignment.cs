using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class ServiceAssignment
{
    [Key]
    [Column("AssignmentID")]
    public int AssignmentId { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("ServiceID")]
    public int ServiceId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    [Column("HotelID")]
    public int? HotelId { get; set; }

    [Column("TransportProviderID")]
    public int? TransportProviderId { get; set; }

    public DateOnly? ServiceDate { get; set; }

    [StringLength(30)]
    public string? ServiceStatus { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CostAmount { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("ServiceAssignments")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("HotelId")]
    [InverseProperty("ServiceAssignments")]
    public virtual Hotel? Hotel { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("ServiceAssignments")]
    public virtual Service Service { get; set; } = null!;

    [ForeignKey("SupplierId")]
    [InverseProperty("ServiceAssignments")]
    public virtual Supplier? Supplier { get; set; }

    [ForeignKey("TransportProviderId")]
    [InverseProperty("ServiceAssignments")]
    public virtual TransportProvider? TransportProvider { get; set; }
}
