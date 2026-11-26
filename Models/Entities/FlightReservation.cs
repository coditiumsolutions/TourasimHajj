using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismHajj.Models.Entities;

[Table("FlightReservations")]
public class FlightReservation
{
    [Key]
    public int FlightReservationId { get; set; }

    [Required, StringLength(40)]
    public string Provider { get; set; } = "Amadeus";

    [StringLength(80)]
    public string? SupplierBookingReference { get; set; }

    [StringLength(40)]
    public string? ClientReference { get; set; }

    [StringLength(40)]
    public string? TicketNumber { get; set; }

    [StringLength(10)]
    public string Origin { get; set; } = string.Empty;

    [StringLength(10)]
    public string Destination { get; set; } = string.Empty;

    [StringLength(20)]
    public string FlightNumber { get; set; } = string.Empty;

    [StringLength(80)]
    public string? CarrierName { get; set; }

    public DateTime DepartureAt { get; set; }

    public DateTime ArrivalAt { get; set; }

    public bool IsRoundTrip { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SellingAmount { get; set; }

    [StringLength(10)]
    public string? Currency { get; set; }

    [Required, StringLength(30)]
    public string Status { get; set; } = "PENDING";

    [StringLength(40)]
    public string? PaymentMethod { get; set; }

    [StringLength(30)]
    public string? PaymentStatus { get; set; }

    [StringLength(80)]
    public string? TravelerFirstName { get; set; }

    [StringLength(80)]
    public string? TravelerLastName { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public string? OfferSummary { get; set; }

    public string? SupplierRawJson { get; set; }

    public int? CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}
