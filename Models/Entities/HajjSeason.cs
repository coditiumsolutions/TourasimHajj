using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class HajjSeason
{
    [Key]
    [Column("HajjSeasonID")]
    public int HajjSeasonId { get; set; }

    [StringLength(100)]
    public string SeasonName { get; set; } = null!;

    public int HajjYear { get; set; }

    public DateOnly? RegistrationStartDate { get; set; }

    public DateOnly? RegistrationEndDate { get; set; }

    public DateOnly? DepartureStartDate { get; set; }

    public DateOnly? DepartureEndDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("HajjSeason")]
    public virtual ICollection<PilgrimGroup> PilgrimGroups { get; set; } = new List<PilgrimGroup>();
}
