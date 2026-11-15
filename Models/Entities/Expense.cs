using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Expense
{
    [Key]
    [Column("ExpenseID")]
    public int ExpenseId { get; set; }

    [Column("BookingID")]
    public int? BookingId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    [StringLength(100)]
    public string? ExpenseCategory { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    public string? ReferenceNo { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Expenses")]
    public virtual Booking? Booking { get; set; }

    [ForeignKey("SupplierId")]
    [InverseProperty("Expenses")]
    public virtual Supplier? Supplier { get; set; }
}
