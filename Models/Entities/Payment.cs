using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(30)]
    public string? PaymentType { get; set; }

    [StringLength(30)]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    public string? ReferenceNo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; }

    [StringLength(100)]
    public string? ReceivedBy { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Payments")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
