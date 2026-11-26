using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismHajj.Models.Entities;

[Table("HotelReservations")]
public class HotelReservation
{
    [Key]
    public int HotelReservationId { get; set; }

    [Required, StringLength(40)]
    public string Provider { get; set; } = "Hotelbeds";

    [StringLength(80)]
    public string? SupplierBookingReference { get; set; }

    [StringLength(40)]
    public string? ClientReference { get; set; }

    [StringLength(40)]
    public string? HotelCode { get; set; }

    [Required, StringLength(200)]
    public string HotelName { get; set; } = string.Empty;

    [StringLength(100)]
    public string Destination { get; set; } = string.Empty;

    [StringLength(150)]
    public string? RoomName { get; set; }

    [StringLength(80)]
    public string? BoardName { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public int Rooms { get; set; } = 1;

    public int Adults { get; set; } = 2;

    public int Children { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NetAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SellingAmount { get; set; }

    [StringLength(10)]
    public string? Currency { get; set; }

    [Required, StringLength(30)]
    public string Status { get; set; } = "PENDING";

    [StringLength(80)]
    public string? HolderFirstName { get; set; }

    [StringLength(80)]
    public string? HolderLastName { get; set; }

    [StringLength(150)]
    public string? HolderEmail { get; set; }

    [StringLength(30)]
    public string? HolderPhone { get; set; }

    public string? GuestsJson { get; set; }

    public string? SupplierRawJson { get; set; }

    public int? CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}
