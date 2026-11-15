using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var passportLimit = DateOnly.FromDateTime(today.AddDays(90));

        var vm = new DashboardViewModel
        {
            TotalCustomers = await _db.Customers.CountAsync(c => c.IsActive != false),
            TotalTravelers = await _db.Travelers.CountAsync(),
            TotalBookings = await _db.Bookings.CountAsync(b => b.BookingStatus != "Cancelled"),
            TodaysBookings = await _db.Bookings.CountAsync(b => b.BookingDate != null && b.BookingDate.Value.Date == today),
            TotalTourismPackages = await _db.TravelPackages.CountAsync(p => p.IsActive != false),
            TotalHajjPilgrims = await _db.BookingTravelers
                .Include(bt => bt.Booking).ThenInclude(b => b.Package)
                .CountAsync(bt => bt.Booking.Package.PackageType == "Hajj" && bt.Booking.BookingStatus != "Cancelled"),
            TotalUmrahPilgrims = await _db.BookingTravelers
                .Include(bt => bt.Booking).ThenInclude(b => b.Package)
                .CountAsync(bt => bt.Booking.Package.PackageType == "Umrah" && bt.Booking.BookingStatus != "Cancelled"),
            TotalRevenue = await _db.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0m,
            OutstandingPayments = await _db.Bookings
                .Where(b => b.BookingStatus != "Cancelled")
                .SumAsync(b => (decimal?)b.BalanceAmount) ?? 0m,
            TotalExpenses = await _db.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0m
        };

        var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
        var bookings = await _db.Bookings
            .Where(b => b.BookingDate >= startMonth && b.BookingStatus != "Cancelled")
            .Select(b => new { b.BookingDate, b.NetAmount, b.PackageId })
            .ToListAsync();

        var payments = await _db.Payments
            .Where(p => p.PaymentDate >= startMonth)
            .Select(p => new { p.PaymentDate, p.Amount })
            .ToListAsync();

        for (var i = 0; i < 12; i++)
        {
            var month = startMonth.AddMonths(i);
            var label = month.ToString("MMM yyyy");
            vm.MonthlyBookings.Add(new ChartPoint
            {
                Label = label,
                Value = bookings.Count(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == month.Year && b.BookingDate.Value.Month == month.Month)
            });
            vm.MonthlyRevenue.Add(new ChartPoint
            {
                Label = label,
                Value = payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == month.Year && p.PaymentDate.Value.Month == month.Month)
                    .Sum(p => p.Amount)
            });
            vm.PaymentCollections.Add(new ChartPoint
            {
                Label = label,
                Value = payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == month.Year && p.PaymentDate.Value.Month == month.Month)
                    .Sum(p => p.Amount)
            });
        }

        vm.PackageWiseSales = await _db.Bookings
            .Where(b => b.BookingStatus != "Cancelled")
            .GroupBy(b => b.Package.PackageName)
            .Select(g => new ChartPoint { Label = g.Key, Value = g.Sum(x => x.NetAmount ?? 0) })
            .OrderByDescending(x => x.Value)
            .Take(8)
            .ToListAsync();

        vm.BookingTypeSplit = await _db.Bookings
            .Where(b => b.BookingStatus != "Cancelled")
            .GroupBy(b => b.Package.PackageType)
            .Select(g => new ChartPoint { Label = g.Key, Value = g.Count() })
            .ToListAsync();

        vm.RecentBookings = await _db.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Package)
            .OrderByDescending(b => b.BookingDate)
            .Take(8)
            .Select(b => new RecentBookingItem
            {
                BookingId = b.BookingId,
                BookingNo = b.BookingNo,
                CustomerName = b.Customer.CustomerName,
                PackageName = b.Package.PackageName,
                BookingDate = b.BookingDate,
                Status = b.BookingStatus,
                NetAmount = b.NetAmount
            }).ToListAsync();

        vm.UpcomingDepartures = await _db.PackageDepartures
            .Include(d => d.Package)
            .Where(d => d.DepartureDate >= DateOnly.FromDateTime(today))
            .OrderBy(d => d.DepartureDate)
            .Take(8)
            .Select(d => new UpcomingDepartureItem
            {
                DepartureId = d.DepartureId,
                PackageName = d.Package.PackageName,
                DepartureDate = d.DepartureDate,
                AvailableSeats = d.AvailableSeats,
                Status = d.DepartureStatus
            }).ToListAsync();

        vm.PendingPayments = await _db.Bookings
            .Include(b => b.Customer)
            .Where(b => b.BookingStatus != "Cancelled" && (b.BalanceAmount ?? 0) > 0)
            .OrderByDescending(b => b.BalanceAmount)
            .Take(8)
            .Select(b => new PendingPaymentItem
            {
                BookingId = b.BookingId,
                BookingNo = b.BookingNo,
                CustomerName = b.Customer.CustomerName,
                Balance = b.BalanceAmount ?? 0
            }).ToListAsync();

        vm.PendingVisas = await _db.VisaApplications
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Where(v => v.VisaStatus != "Approved" && v.VisaStatus != "Rejected")
            .OrderByDescending(v => v.ApplicationDate)
            .Take(8)
            .Select(v => new PendingVisaItem
            {
                VisaApplicationId = v.VisaApplicationId,
                TravelerName = v.BookingTraveler.Traveler.FullName,
                VisaType = v.VisaType,
                VisaStatus = v.VisaStatus
            }).ToListAsync();

        vm.PassportAlerts = await _db.Travelers
            .Where(t => t.PassportExpiryDate != null && t.PassportExpiryDate <= passportLimit)
            .OrderBy(t => t.PassportExpiryDate)
            .Take(8)
            .Select(t => new PassportAlertItem
            {
                TravelerId = t.TravelerId,
                FullName = t.FullName,
                PassportNo = t.PassportNo,
                ExpiryDate = t.PassportExpiryDate
            }).ToListAsync();

        vm.UpcomingHajjDepartures = await _db.PackageDepartures
            .Include(d => d.Package)
            .Where(d => d.Package.PackageType == "Hajj" && d.DepartureDate >= DateOnly.FromDateTime(today))
            .OrderBy(d => d.DepartureDate)
            .Take(8)
            .Select(d => new UpcomingDepartureItem
            {
                DepartureId = d.DepartureId,
                PackageName = d.Package.PackageName,
                DepartureDate = d.DepartureDate,
                AvailableSeats = d.AvailableSeats,
                Status = d.DepartureStatus
            }).ToListAsync();

        vm.RecentCustomers = await _db.Customers
            .OrderByDescending(c => c.CreatedDate)
            .Take(8)
            .Select(c => new RecentCustomerItem
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                MobileNo = c.MobileNo,
                CreatedDate = c.CreatedDate
            }).ToListAsync();

        return vm;
    }
}
