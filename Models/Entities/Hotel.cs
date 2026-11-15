using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Hotel
{
    [Key]
    [Column("HotelID")]
    public int HotelId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    [Column("CityID")]
    public int CityId { get; set; }

    [StringLength(150)]
    public string HotelName { get; set; } = null!;

    public int? StarRating { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Column("DistanceFromHaramKM", TypeName = "decimal(10, 2)")]
    public decimal? DistanceFromHaramKm { get; set; }

    [Column("DistanceFromNabawiKM", TypeName = "decimal(10, 2)")]
    public decimal? DistanceFromNabawiKm { get; set; }

    [StringLength(30)]
    public string? ContactNo { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Hotels")]
    public virtual City City { get; set; } = null!;

    [InverseProperty("Hotel")]
    public virtual ICollection<RoomAllocation> RoomAllocations { get; set; } = new List<RoomAllocation>();

    [InverseProperty("Hotel")]
    public virtual ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Hotels")]
    public virtual Supplier? Supplier { get; set; }
}
