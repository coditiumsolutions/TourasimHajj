using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Refund
{
    [Key]
    [Column("RefundID")]
    public int RefundId { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("PaymentID")]
    public int? PaymentId { get; set; }

    public DateOnly? RefundDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal RefundAmount { get; set; }

    [StringLength(500)]
    public string? RefundReason { get; set; }

    [StringLength(30)]
    public string? RefundStatus { get; set; }

    [StringLength(30)]
    public string? PaymentMethod { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Refunds")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("Refunds")]
    public virtual Payment? Payment { get; set; }
}
