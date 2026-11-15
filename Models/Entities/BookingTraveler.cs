using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class BookingTraveler
{
    [Key]
    [Column("BookingTravelerID")]
    public int BookingTravelerId { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("TravelerID")]
    public int TravelerId { get; set; }

    [StringLength(20)]
    public string? TravelerCategory { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PackagePrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NetAmount { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingTravelers")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("BookingTraveler")]
    public virtual ICollection<PilgrimGroupMember> PilgrimGroupMembers { get; set; } = new List<PilgrimGroupMember>();

    [InverseProperty("BookingTraveler")]
    public virtual ICollection<RoomAllocation> RoomAllocations { get; set; } = new List<RoomAllocation>();

    [ForeignKey("TravelerId")]
    [InverseProperty("BookingTravelers")]
    public virtual Traveler Traveler { get; set; } = null!;

    [InverseProperty("BookingTraveler")]
    public virtual ICollection<VisaApplication> VisaApplications { get; set; } = new List<VisaApplication>();
}
