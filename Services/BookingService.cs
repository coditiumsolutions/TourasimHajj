using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public BookingService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<(List<Booking> Items, int Total)> GetPagedAsync(string? q, string? type, string? status, int page, int pageSize)
    {
        var query = _db.Bookings.Include(b => b.Customer).Include(b => b.Package).Include(b => b.Departure).AsQueryable();
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(b => b.Package.PackageType == type || b.BookingType == type);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(b => b.BookingStatus == status);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(b => b.BookingNo.Contains(q) || b.Customer.CustomerName.Contains(q));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.BookingId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Booking?> GetByIdAsync(int id) =>
        _db.Bookings.Include(b => b.Customer).Include(b => b.Package).Include(b => b.Departure)
            .FirstOrDefaultAsync(b => b.BookingId == id);

    public async Task<BookingDetailsViewModel?> GetDetailsAsync(int id)
    {
        var booking = await GetByIdAsync(id);
        if (booking == null) return null;
        return new BookingDetailsViewModel
        {
            Booking = booking,
            Travelers = await _db.BookingTravelers.Include(bt => bt.Traveler).Where(bt => bt.BookingId == id).ToListAsync(),
            Payments = await _db.Payments.Where(p => p.BookingId == id).OrderByDescending(p => p.PaymentDate).ToListAsync()
        };
    }

    public async Task<(bool Ok, string Message, int? BookingId)> CreateBookingAsync(BookingCreateViewModel model, string? userId)
    {
        if (model.TravelerIds == null || model.TravelerIds.Count == 0)
            return (false, "Select at least one traveler.", null);

        var package = await _db.TravelPackages.FirstOrDefaultAsync(p => p.PackageId == model.PackageId);
        if (package == null) return (false, "Package not found.", null);

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            PackageDeparture? departure = null;
            if (model.DepartureId.HasValue)
            {
                departure = await _db.PackageDepartures.FirstOrDefaultAsync(d => d.DepartureId == model.DepartureId.Value);
                if (departure == null) return (false, "Departure not found.", null);

                var seatsNeeded = model.TotalAdults + model.TotalChildren + model.TotalInfants;
                if ((departure.AvailableSeats ?? 0) < seatsNeeded)
                    return (false, "Not enough available seats for this departure.", null);

                // Passport expiry check for international / Hajj / Umrah
                if (package.PackageType is "International" or "Hajj" or "Umrah")
                {
                    var minExpiry = departure.DepartureDate.AddMonths(6);
                    var travelers = await _db.Travelers.Where(t => model.TravelerIds.Contains(t.TravelerId)).ToListAsync();
                    var invalid = travelers.Where(t => t.PassportExpiryDate == null || t.PassportExpiryDate < minExpiry).ToList();
                    if (invalid.Count > 0)
                        return (false, $"Passport expiry too soon for: {string.Join(", ", invalid.Select(t => t.FullName))}", null);
                }
            }

            var adultPrice = departure?.AdultPrice ?? package.AdultPrice ?? 0;
            var childPrice = departure?.ChildPrice ?? package.ChildPrice ?? 0;
            var infantPrice = package.InfantPrice ?? 0;
            var total = (adultPrice * model.TotalAdults) + (childPrice * model.TotalChildren) + (infantPrice * model.TotalInfants);
            var net = total - model.DiscountAmount + model.TaxAmount;
            if (net < 0) return (false, "Net amount cannot be negative.", null);

            var bookingNo = await GenerateBookingNoAsync();
            var booking = new Booking
            {
                BookingNo = bookingNo,
                CustomerId = model.CustomerId,
                PackageId = model.PackageId,
                DepartureId = model.DepartureId,
                BookingDate = DateTime.Now,
                BookingType = model.BookingType ?? package.PackageType,
                TotalAdults = model.TotalAdults,
                TotalChildren = model.TotalChildren,
                TotalInfants = model.TotalInfants,
                TotalAmount = total,
                DiscountAmount = model.DiscountAmount,
                TaxAmount = model.TaxAmount,
                NetAmount = net,
                PaidAmount = 0,
                BalanceAmount = net,
                BookingStatus = "Confirmed",
                PaymentStatus = "Unpaid",
                Remarks = model.Remarks
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            foreach (var travelerId in model.TravelerIds.Distinct())
            {
                var traveler = await _db.Travelers.FindAsync(travelerId);
                if (traveler == null) continue;
                var category = "Adult";
                var price = adultPrice;
                _db.BookingTravelers.Add(new BookingTraveler
                {
                    BookingId = booking.BookingId,
                    TravelerId = travelerId,
                    TravelerCategory = category,
                    PackagePrice = price,
                    DiscountAmount = 0,
                    NetAmount = price
                });
            }

            if (departure != null)
            {
                var seatsNeeded = model.TotalAdults + model.TotalChildren + model.TotalInfants;
                departure.AvailableSeats = (departure.AvailableSeats ?? 0) - seatsNeeded;
                _db.PackageDepartures.Update(departure);
            }

            if (model.InitialPayment > 0)
            {
                if (model.InitialPayment > net)
                    return (false, "Initial payment cannot exceed net amount.", null);

                _db.Payments.Add(new Payment
                {
                    BookingId = booking.BookingId,
                    PaymentDate = DateTime.Now,
                    PaymentType = "Initial",
                    PaymentMethod = model.PaymentMethod ?? "Cash",
                    Amount = model.InitialPayment,
                    CurrencyCode = package.CurrencyCode ?? departure?.CurrencyCode ?? "PKR",
                    ReceivedBy = userId
                });
            }

            await _db.SaveChangesAsync();
            await RecalculatePaymentTotalsAsync(booking.BookingId);
            await _audit.LogAsync(userId, "Bookings", "Bookings", booking.BookingId.ToString(), "Create");
            await tx.CommitAsync();
            return (true, "Booking created successfully.", booking.BookingId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Ok, string Message)> CancelAsync(int bookingId, string? userId)
    {
        var booking = await _db.Bookings.Include(b => b.Departure).FirstOrDefaultAsync(b => b.BookingId == bookingId);
        if (booking == null) return (false, "Booking not found.");
        if (booking.BookingStatus == "Cancelled") return (false, "Booking already cancelled.");

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var seats = (booking.TotalAdults ?? 0) + (booking.TotalChildren ?? 0) + (booking.TotalInfants ?? 0);
            if (booking.DepartureId.HasValue)
            {
                var dep = await _db.PackageDepartures.FindAsync(booking.DepartureId.Value);
                if (dep != null)
                {
                    dep.AvailableSeats = (dep.AvailableSeats ?? 0) + seats;
                    _db.PackageDepartures.Update(dep);
                }
            }
            booking.BookingStatus = "Cancelled";
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
            await _audit.LogAsync(userId, "Bookings", "Bookings", bookingId.ToString(), "Cancel");
            await tx.CommitAsync();
            return (true, "Booking cancelled.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    public async Task RecalculatePaymentTotalsAsync(int bookingId)
    {
        var booking = await _db.Bookings.FindAsync(bookingId);
        if (booking == null) return;
        var paid = await _db.Payments.Where(p => p.BookingId == bookingId).SumAsync(p => (decimal?)p.Amount) ?? 0;
        var refunded = await _db.Refunds.Where(r => r.BookingId == bookingId && r.RefundStatus != "Rejected")
            .SumAsync(r => (decimal?)r.RefundAmount) ?? 0;
        var netPaid = paid - refunded;
        booking.PaidAmount = netPaid;
        booking.BalanceAmount = (booking.NetAmount ?? 0) - netPaid;
        booking.PaymentStatus = booking.BalanceAmount <= 0 ? "Paid" : (netPaid > 0 ? "Partial" : "Unpaid");
        _db.Bookings.Update(booking);
        await _db.SaveChangesAsync();
    }

    private async Task<string> GenerateBookingNoAsync()
    {
        var prefix = $"BK-{DateTime.Now:yyyyMMdd}-";
        var count = await _db.Bookings.CountAsync(b => b.BookingNo.StartsWith(prefix));
        return $"{prefix}{(count + 1):D4}";
    }
}
