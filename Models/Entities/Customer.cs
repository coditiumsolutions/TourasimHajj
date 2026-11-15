using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class Customer
{
    [Key]
    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [StringLength(150)]
    public string CustomerName { get; set; } = null!;

    [StringLength(150)]
    public string? FatherName { get; set; }

    [StringLength(30)]
    public string? MobileNo { get; set; }

    [StringLength(30)]
    public string? WhatsAppNo { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [Column("CNIC")]
    [StringLength(30)]
    public string? Cnic { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Column("CityID")]
    public int? CityId { get; set; }

    [StringLength(30)]
    public string? CustomerType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CityId")]
    [InverseProperty("Customers")]
    public virtual City? City { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Traveler> Travelers { get; set; } = new List<Traveler>();
}
