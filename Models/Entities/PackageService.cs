using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class PackageService
{
    [Key]
    [Column("PackageServiceID")]
    public int PackageServiceId { get; set; }

    [Column("PackageID")]
    public int PackageId { get; set; }

    [Column("ServiceID")]
    public int ServiceId { get; set; }

    public bool? IsIncluded { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AdditionalPrice { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    [ForeignKey("PackageId")]
    [InverseProperty("PackageServices")]
    public virtual TravelPackage Package { get; set; } = null!;

    [ForeignKey("ServiceId")]
    [InverseProperty("PackageServices")]
    public virtual Service Service { get; set; } = null!;
}
