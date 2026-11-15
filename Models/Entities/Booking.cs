using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

[Index("BookingNo", Name = "UQ__Bookings__739788AB39122169", IsUnique = true)]
public partial class Booking
{
    [Key]
    [Column("BookingID")]
    public int BookingId { get; set; }

    [StringLength(30)]
    public string BookingNo { get; set; } = null!;

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Column("PackageID")]
    public int PackageId { get; set; }

    [Column("DepartureID")]
    public int? DepartureId { get; set; }

    public DateTime? BookingDate { get; set; }

    [StringLength(30)]
    public string? BookingType { get; set; }

    public int? TotalAdults { get; set; }

    public int? TotalChildren { get; set; }

    public int? TotalInfants { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TaxAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NetAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PaidAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BalanceAmount { get; set; }

    [StringLength(30)]
    public string? BookingStatus { get; set; }

    [StringLength(30)]
    public string? PaymentStatus { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<BookingTraveler> BookingTravelers { get; set; } = new List<BookingTraveler>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Bookings")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("DepartureId")]
    [InverseProperty("Bookings")]
    public virtual PackageDeparture? Departure { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [ForeignKey("PackageId")]
    [InverseProperty("Bookings")]
    public virtual TravelPackage Package { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Booking")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [InverseProperty("Booking")]
    public virtual ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();

    [InverseProperty("Booking")]
    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();
}
