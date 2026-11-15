using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Service
{
    [Key]
    [Column("ServiceID")]
    public int ServiceId { get; set; }

    [StringLength(150)]
    public string ServiceName { get; set; } = null!;

    [StringLength(50)]
    public string? ServiceCategory { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Service")]
    public virtual ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();

    [InverseProperty("Service")]
    public virtual ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();
}
