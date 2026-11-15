using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class SupplierPayment
{
    [Key]
    [Column("SupplierPaymentID")]
    public int SupplierPaymentId { get; set; }

    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [Column("BookingID")]
    public int? BookingId { get; set; }

    public DateOnly PaymentDate { get; set; }

    [StringLength(30)]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    public string? ReferenceNo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("SupplierPayments")]
    public virtual Booking? Booking { get; set; }

    [ForeignKey("SupplierId")]
    [InverseProperty("SupplierPayments")]
    public virtual Supplier Supplier { get; set; } = null!;
}
