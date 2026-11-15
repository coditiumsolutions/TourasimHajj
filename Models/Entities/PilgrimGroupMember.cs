using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

[Index("PilgrimGroupId", "BookingTravelerId", Name = "UQ_PilgrimGroupMembers", IsUnique = true)]
public partial class PilgrimGroupMember
{
    [Key]
    [Column("PilgrimGroupMemberID")]
    public int PilgrimGroupMemberId { get; set; }

    [Column("PilgrimGroupID")]
    public int PilgrimGroupId { get; set; }

    [Column("BookingTravelerID")]
    public int BookingTravelerId { get; set; }

    public bool? IsGroupLeader { get; set; }

    [StringLength(50)]
    public string? RoomGroup { get; set; }

    [StringLength(50)]
    public string? TransportGroup { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingTravelerId")]
    [InverseProperty("PilgrimGroupMembers")]
    public virtual BookingTraveler BookingTraveler { get; set; } = null!;

    [ForeignKey("PilgrimGroupId")]
    [InverseProperty("PilgrimGroupMembers")]
    public virtual PilgrimGroup PilgrimGroup { get; set; } = null!;
}
