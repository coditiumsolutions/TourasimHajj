using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

[Index("GroupCode", Name = "UQ__PilgrimG__3B97438051338533", IsUnique = true)]
public partial class PilgrimGroup
{
    [Key]
    [Column("PilgrimGroupID")]
    public int PilgrimGroupId { get; set; }

    [Column("HajjSeasonID")]
    public int HajjSeasonId { get; set; }

    [Column("DepartureID")]
    public int? DepartureId { get; set; }

    [StringLength(30)]
    public string GroupCode { get; set; } = null!;

    [StringLength(150)]
    public string? GroupName { get; set; }

    [StringLength(150)]
    public string? GroupLeaderName { get; set; }

    [StringLength(30)]
    public string? GroupLeaderMobile { get; set; }

    public int? TotalPilgrims { get; set; }

    [StringLength(30)]
    public string? GroupStatus { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("DepartureId")]
    [InverseProperty("PilgrimGroups")]
    public virtual PackageDeparture? Departure { get; set; }

    [ForeignKey("HajjSeasonId")]
    [InverseProperty("PilgrimGroups")]
    public virtual HajjSeason HajjSeason { get; set; } = null!;

    [InverseProperty("PilgrimGroup")]
    public virtual ICollection<PilgrimGroupMember> PilgrimGroupMembers { get; set; } = new List<PilgrimGroupMember>();
}
