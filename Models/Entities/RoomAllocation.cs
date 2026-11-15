using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class RoomAllocation
{
    [Key]
    [Column("RoomAllocationID")]
    public int RoomAllocationId { get; set; }

    [Column("BookingTravelerID")]
    public int BookingTravelerId { get; set; }

    [Column("HotelID")]
    public int HotelId { get; set; }

    [StringLength(50)]
    public string? RoomNumber { get; set; }

    [StringLength(50)]
    public string? RoomType { get; set; }

    public DateOnly? CheckInDate { get; set; }

    public DateOnly? CheckOutDate { get; set; }

    [StringLength(30)]
    public string? SharingType { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingTravelerId")]
    [InverseProperty("RoomAllocations")]
    public virtual BookingTraveler BookingTraveler { get; set; } = null!;

    [ForeignKey("HotelId")]
    [InverseProperty("RoomAllocations")]
    public virtual Hotel Hotel { get; set; } = null!;
}
