using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class CustomerPortalService : ICustomerPortalService
{
    private readonly ApplicationDbContext _db;
    private readonly IBookingService _bookings;

    public CustomerPortalService(ApplicationDbContext db, IBookingService bookings)
    {
        _db = db;
        _bookings = bookings;
    }

    public Task<Customer?> GetCustomerByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
        _db.Customers.FirstOrDefaultAsync(c => c.AspNetUserId == userId && c.IsActive != false, cancellationToken);

    public async Task<CustomerPortalDashboardViewModel> GetDashboardAsync(string userId, CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerByUserIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("Customer profile not found.");

        var bookings = await _db.Bookings
            .Include(b => b.Package)
            .Include(b => b.Departure)
            .Where(b => b.CustomerId == customer.CustomerId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var upcoming = bookings
            .Where(b => b.Departure != null && b.Departure.DepartureDate >= today && b.BookingStatus != "Cancelled")
            .ToList();

        var bookingIds = bookings.Select(b => b.BookingId).ToList();
        var payments = await _db.Payments
            .Where(p => bookingIds.Contains(p.BookingId))
            .OrderByDescending(p => p.PaymentDate)
            .Take(8)
            .ToListAsync(cancellationToken);

        var travelerIds = await _db.Travelers
            .Where(t => t.CustomerId == customer.CustomerId)
            .Select(t => t.TravelerId)
            .ToListAsync(cancellationToken);

        var docs = await _db.TravelerDocuments
            .Include(d => d.Traveler)
            .Where(d => travelerIds.Contains(d.TravelerId))
            .OrderByDescending(d => d.UploadedDate)
            .Take(8)
            .ToListAsync(cancellationToken);

        return new CustomerPortalDashboardViewModel
        {
            CustomerName = customer.CustomerName,
            TotalBookings = bookings.Count,
            UpcomingTrips = upcoming.Count,
            PendingPayments = bookings.Count(b => (b.BalanceAmount ?? 0) > 0),
            CompletedTrips = bookings.Count(b => b.BookingStatus == "Completed" || b.BookingStatus == "Closed"),
            RecentBookings = bookings.Take(5).ToList(),
            UpcomingDepartures = upcoming.Select(b => b.Departure!).DistinctBy(d => d.DepartureId).Take(5).ToList(),
            RecentPayments = payments,
            PendingDocuments = docs
        };
    }

    public async Task<CustomerProfileViewModel?> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return null;
        return new CustomerProfileViewModel
        {
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            FatherName = c.FatherName,
            MobileNo = c.MobileNo,
            WhatsAppNo = c.WhatsAppNo,
            Email = c.Email,
            Cnic = c.Cnic,
            Address = c.Address
        };
    }

    public async Task<(bool Ok, string Message)> UpdateProfileAsync(string userId, CustomerProfileViewModel model, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return (false, "Customer profile not found.");

        if (!string.IsNullOrWhiteSpace(model.MobileNo))
        {
            var dupMobile = await _db.Customers.AnyAsync(x =>
                x.CustomerId != c.CustomerId && x.MobileNo == model.MobileNo, cancellationToken);
            if (dupMobile) return (false, "Mobile number already in use.");
        }

        c.CustomerName = model.CustomerName.Trim();
        c.FatherName = model.FatherName;
        c.MobileNo = model.MobileNo;
        c.WhatsAppNo = model.WhatsAppNo;
        c.Email = model.Email;
        c.Cnic = model.Cnic;
        c.Address = model.Address;
        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Profile updated.");
    }

    public async Task<List<Traveler>> GetTravelersAsync(string userId, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return [];
        return await _db.Travelers
            .Include(t => t.TravelerDocuments)
            .Where(t => t.CustomerId == c.CustomerId)
            .OrderBy(t => t.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Ok, string Message)> SaveTravelerAsync(string userId, Traveler traveler, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return (false, "Customer profile not found.");

        if (traveler.TravelerId == 0)
        {
            traveler.CustomerId = c.CustomerId;
            traveler.CreatedDate = DateTime.Now;
            _db.Travelers.Add(traveler);
        }
        else
        {
            var existing = await _db.Travelers.FirstOrDefaultAsync(t =>
                t.TravelerId == traveler.TravelerId && t.CustomerId == c.CustomerId, cancellationToken);
            if (existing == null) return (false, "Traveler not found.");
            existing.FullName = traveler.FullName;
            existing.FatherName = traveler.FatherName;
            existing.Gender = traveler.Gender;
            existing.DateOfBirth = traveler.DateOfBirth;
            existing.Cnic = traveler.Cnic;
            existing.PassportNo = traveler.PassportNo;
            existing.PassportIssueDate = traveler.PassportIssueDate;
            existing.PassportExpiryDate = traveler.PassportExpiryDate;
            existing.MobileNo = traveler.MobileNo;
            existing.Relationship = traveler.Relationship;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Traveler saved.");
    }

    public async Task<List<Booking>> GetBookingsAsync(string userId, string? typeFilter = null, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return [];
        var query = _db.Bookings
            .Include(b => b.Package)
            .Include(b => b.Departure)
            .Where(b => b.CustomerId == c.CustomerId);
        if (!string.IsNullOrWhiteSpace(typeFilter))
            query = query.Where(b => b.Package.PackageType == typeFilter || b.BookingType == typeFilter);
        return await query.OrderByDescending(b => b.BookingDate).ToListAsync(cancellationToken);
    }

    public async Task<BookingDetailsViewModel?> GetBookingDetailsAsync(string userId, int bookingId, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return null;
        var details = await _bookings.GetDetailsAsync(bookingId);
        if (details?.Booking == null || details.Booking.CustomerId != c.CustomerId) return null;
        return details;
    }

    public async Task<List<Payment>> GetPaymentsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return [];
        var bookingIds = await _db.Bookings.Where(b => b.CustomerId == c.CustomerId).Select(b => b.BookingId).ToListAsync(cancellationToken);
        return await _db.Payments
            .Include(p => p.Booking)
            .Where(p => bookingIds.Contains(p.BookingId))
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TravelerDocument>> GetDocumentsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var travelers = await GetTravelersAsync(userId, cancellationToken);
        var ids = travelers.Select(t => t.TravelerId).ToList();
        return await _db.TravelerDocuments
            .Include(d => d.Traveler)
            .Where(d => ids.Contains(d.TravelerId))
            .OrderByDescending(d => d.UploadedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerBookingRequestViewModel?> GetBookingRequestAsync(string userId, int packageId, CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return null;
        var package = await _db.TravelPackages.FirstOrDefaultAsync(p => p.PackageId == packageId && p.IsActive != false, cancellationToken);
        if (package == null) return null;

        var today = DateOnly.FromDateTime(DateTime.Today);
        return new CustomerBookingRequestViewModel
        {
            PackageId = package.PackageId,
            PackageName = package.PackageName,
            TravelerOptions = await _db.Travelers.Where(t => t.CustomerId == c.CustomerId).OrderBy(t => t.FullName).ToListAsync(cancellationToken),
            DepartureOptions = await _db.PackageDepartures
                .Where(d => d.PackageId == packageId && d.DepartureDate >= today && (d.AvailableSeats ?? 0) > 0)
                .OrderBy(d => d.DepartureDate)
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<(bool Ok, string Message, int? BookingId)> CreateBookingAsync(
        string userId,
        CustomerBookingRequestViewModel model,
        CancellationToken cancellationToken = default)
    {
        var c = await GetCustomerByUserIdAsync(userId, cancellationToken);
        if (c == null) return (false, "Customer profile not found.", null);

        var packageType = await _db.TravelPackages.AsNoTracking()
            .Where(p => p.PackageId == model.PackageId)
            .Select(p => p.PackageType)
            .FirstOrDefaultAsync(cancellationToken);
        if (string.Equals(packageType, "Hajj", StringComparison.OrdinalIgnoreCase))
            return (false, "Hajj packages cannot be booked online. Please register interest via Contact — arrangements are made through licensed Ministry / Nusuk campaigns.", null);

        var owned = await _db.Travelers
            .Where(t => t.CustomerId == c.CustomerId && model.TravelerIds.Contains(t.TravelerId))
            .Select(t => t.TravelerId)
            .ToListAsync(cancellationToken);
        if (owned.Count != model.TravelerIds.Count)
            return (false, "One or more travelers do not belong to your account.", null);

        var create = new BookingCreateViewModel
        {
            CustomerId = c.CustomerId,
            PackageId = model.PackageId,
            DepartureId = model.DepartureId,
            TotalAdults = model.TotalAdults,
            TotalChildren = model.TotalChildren,
            TotalInfants = model.TotalInfants,
            TravelerIds = model.TravelerIds
        };

        return await _bookings.CreateBookingAsync(create, userId);
    }
}
