using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Employee
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [StringLength(150)]
    public string EmployeeName { get; set; } = null!;

    [StringLength(100)]
    public string? Designation { get; set; }

    [StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    public DateOnly? JoiningDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Salary { get; set; }

    public bool? IsActive { get; set; }
}
