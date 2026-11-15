using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;
    private readonly IAccountsService _accounts;
    private readonly IHajjService _hajj;
    private readonly IUmrahService _umrah;

    public ReportService(ApplicationDbContext db, IAccountsService accounts, IHajjService hajj, IUmrahService umrah)
    {
        _db = db;
        _accounts = accounts;
        _hajj = hajj;
        _umrah = umrah;
    }

    public ReportIndexViewModel GetReportMenu() => new()
    {
        Reports =
        [
            new ReportMenuItem { Title = "Booking Register", Action = "BookingRegister", Description = "All bookings with payment status" },
            new ReportMenuItem { Title = "Customer Ledger", Action = "CustomerLedger", Description = "Customer account transactions" },
            new ReportMenuItem { Title = "Supplier Ledger", Action = "SupplierLedger", Description = "Supplier expenses and payments" },
            new ReportMenuItem { Title = "Daily Collection", Action = "DailyCollection", Description = "Payments grouped by day" },
            new ReportMenuItem { Title = "Monthly Collection", Action = "MonthlyCollection", Description = "Payments grouped by month" },
            new ReportMenuItem { Title = "Outstanding Payments", Action = "OutstandingPayments", Description = "Bookings with balance due" },
            new ReportMenuItem { Title = "Package Sales", Action = "PackageSales", Description = "Sales by package" },
            new ReportMenuItem { Title = "Hajj Pilgrim Register", Action = "HajjPilgrimRegister", Description = "Hajj group pilgrims" },
            new ReportMenuItem { Title = "Umrah Pilgrim Register", Action = "UmrahPilgrimRegister", Description = "Umrah pilgrims and rooms" },
            new ReportMenuItem { Title = "Visa Status", Action = "VisaStatus", Description = "Visa application status list" },
            new ReportMenuItem { Title = "Passport Expiry", Action = "PassportExpiry", Description = "Travelers with expiring passports" },
            new ReportMenuItem { Title = "Hotel Room Allocation", Action = "HotelRoomAllocation", Description = "Room assignments" },
            new ReportMenuItem { Title = "Supplier Payments", Action = "SupplierPayments", Description = "Payments made to suppliers" },
            new ReportMenuItem { Title = "Expense Register", Action = "ExpenseRegister", Description = "Operational expenses" },
            new ReportMenuItem { Title = "Profit & Loss", Action = "ProfitLoss", Description = "Revenue vs expenses summary" }
        ]
    };

    public async Task<IReadOnlyList<BookingRegisterRow>> GetBookingRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Bookings.AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.Package)
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(b => b.BookingDate >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var to = filter.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(b => b.BookingDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(b => b.BookingStatus == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter.BookingNo))
            query = query.Where(b => b.BookingNo.Contains(filter.BookingNo));

        return await query.OrderByDescending(b => b.BookingDate)
            .Select(b => new BookingRegisterRow
            {
                BookingNo = b.BookingNo,
                CustomerName = b.Customer.CustomerName,
                PackageName = b.Package.PackageName,
                BookingDate = b.BookingDate,
                BookingStatus = b.BookingStatus,
                NetAmount = b.NetAmount,
                PaidAmount = b.PaidAmount,
                BalanceAmount = b.BalanceAmount,
                PaymentStatus = b.PaymentStatus
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerLedgerRow>> GetCustomerLedgerReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        return await _accounts.GetCustomerLedgerAsync(new LedgerFilterModel
        {
            CustomerId = filter.CustomerId,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierLedgerRow>> GetSupplierLedgerReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        return await _accounts.GetSupplierLedgerAsync(new LedgerFilterModel
        {
            SupplierId = filter.SupplierId,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<CollectionRow>> GetDailyCollectionAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().AsQueryable();
        ApplyPaymentDateFilter(ref query, filter);

        var payments = await query.ToListAsync(cancellationToken);
        return payments
            .Where(p => p.PaymentDate.HasValue)
            .GroupBy(p => DateOnly.FromDateTime(p.PaymentDate!.Value))
            .OrderBy(g => g.Key)
            .Select(g => new CollectionRow
            {
                CollectionDate = g.Key,
                PaymentMethod = null,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(x => x.Amount)
            }).ToList();
    }

    public async Task<IReadOnlyList<MonthlyCollectionRow>> GetMonthlyCollectionAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().AsQueryable();
        ApplyPaymentDateFilter(ref query, filter);

        var payments = await query.Where(p => p.PaymentDate != null).ToListAsync(cancellationToken);
        return payments
            .GroupBy(p => new { p.PaymentDate!.Value.Year, p.PaymentDate!.Value.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyCollectionRow
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthLabel = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(x => x.Amount)
            }).ToList();
    }

    public async Task<IReadOnlyList<OutstandingPaymentRow>> GetOutstandingPaymentsAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Bookings.AsNoTracking()
            .Include(b => b.Customer)
            .Where(b => (b.BalanceAmount ?? 0) > 0 && b.BookingStatus != "Cancelled");

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(b => b.PaymentStatus == filter.Status);

        return await query.OrderByDescending(b => b.BalanceAmount)
            .Select(b => new OutstandingPaymentRow
            {
                BookingNo = b.BookingNo,
                CustomerName = b.Customer.CustomerName,
                MobileNo = b.Customer.MobileNo,
                BookingDate = b.BookingDate,
                NetAmount = b.NetAmount,
                PaidAmount = b.PaidAmount,
                BalanceAmount = b.BalanceAmount,
                PaymentStatus = b.PaymentStatus
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PackageSalesRow>> GetPackageSalesAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Bookings.AsNoTracking()
            .Include(b => b.Package)
            .Where(b => b.BookingStatus != "Cancelled");

        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(b => b.BookingDate >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var to = filter.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(b => b.BookingDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(filter.PackageType))
            query = query.Where(b => b.Package.PackageType == filter.PackageType);

        var bookings = await query.ToListAsync(cancellationToken);
        return bookings.GroupBy(b => b.PackageId)
            .Select(g =>
            {
                var pkg = g.First().Package;
                return new PackageSalesRow
                {
                    PackageCode = pkg.PackageCode,
                    PackageName = pkg.PackageName,
                    PackageType = pkg.PackageType,
                    BookingCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.NetAmount ?? 0m)
                };
            })
            .OrderByDescending(r => r.TotalRevenue)
            .ToList();
    }

    public async Task<IReadOnlyList<HajjPilgrimRegisterRow>> GetHajjPilgrimRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
        => await _hajj.GetHajjPilgrimRegisterAsync(null, null, cancellationToken);

    public async Task<IReadOnlyList<UmrahPilgrimRegisterRow>> GetUmrahPilgrimRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
        => await _umrah.GetUmrahPilgrimRegisterAsync(cancellationToken);

    public async Task<IReadOnlyList<VisaStatusReportRow>> GetVisaStatusAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.VisaApplications.AsNoTracking()
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Booking)
            .Include(v => v.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(v => v.VisaStatus == filter.Status);

        return await query.OrderByDescending(v => v.VisaApplicationId)
            .Select(v => new VisaStatusReportRow
            {
                TravelerName = v.BookingTraveler.Traveler.FullName,
                BookingNo = v.BookingTraveler.Booking.BookingNo,
                VisaType = v.VisaType,
                VisaStatus = v.VisaStatus,
                ApplicationDate = v.ApplicationDate,
                VisaExpiryDate = v.VisaExpiryDate,
                SupplierName = v.Supplier != null ? v.Supplier.SupplierName : null
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PassportExpiryRow>> GetPassportExpiryAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var limit = today.AddDays(filter.PassportExpiryDays > 0 ? filter.PassportExpiryDays : 90);

        return await _db.Travelers.AsNoTracking()
            .Include(t => t.Customer)
            .Where(t => t.PassportExpiryDate != null && t.PassportExpiryDate <= limit)
            .OrderBy(t => t.PassportExpiryDate)
            .Select(t => new PassportExpiryRow
            {
                TravelerName = t.FullName,
                CustomerName = t.Customer.CustomerName,
                PassportNo = t.PassportNo,
                PassportExpiryDate = t.PassportExpiryDate,
                DaysToExpiry = t.PassportExpiryDate.HasValue ? t.PassportExpiryDate.Value.DayNumber - today.DayNumber : 0
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HotelRoomAllocationRow>> GetHotelRoomAllocationAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.RoomAllocations.AsNoTracking()
            .Include(r => r.Hotel).ThenInclude(h => h.City)
            .Include(r => r.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(r => r.BookingTraveler).ThenInclude(bt => bt.Booking)
            .AsQueryable();

        if (filter.FromDate.HasValue)
            query = query.Where(r => r.CheckInDate >= filter.FromDate || r.CheckOutDate >= filter.FromDate);
        if (filter.ToDate.HasValue)
            query = query.Where(r => r.CheckInDate <= filter.ToDate || r.CheckOutDate <= filter.ToDate);

        return await query.OrderBy(r => r.Hotel.HotelName).ThenBy(r => r.RoomNumber)
            .Select(r => new HotelRoomAllocationRow
            {
                HotelName = r.Hotel.HotelName,
                CityName = r.Hotel.City.CityName,
                PilgrimName = r.BookingTraveler.Traveler.FullName,
                BookingNo = r.BookingTraveler.Booking.BookingNo,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierPaymentReportRow>> GetSupplierPaymentsReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.SupplierPayments.AsNoTracking().Include(sp => sp.Supplier).Include(sp => sp.Booking).AsQueryable();
        if (filter.FromDate.HasValue)
            query = query.Where(sp => sp.PaymentDate >= filter.FromDate);
        if (filter.ToDate.HasValue)
            query = query.Where(sp => sp.PaymentDate <= filter.ToDate);
        if (filter.SupplierId.HasValue)
            query = query.Where(sp => sp.SupplierId == filter.SupplierId);

        return await query.OrderByDescending(sp => sp.PaymentDate)
            .Select(sp => new SupplierPaymentReportRow
            {
                PaymentDate = sp.PaymentDate,
                SupplierName = sp.Supplier.SupplierName,
                BookingNo = sp.Booking != null ? sp.Booking.BookingNo : null,
                PaymentMethod = sp.PaymentMethod,
                ReferenceNo = sp.ReferenceNo,
                Amount = sp.Amount
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExpenseRegisterRow>> GetExpenseRegisterAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Expenses.AsNoTracking().Include(e => e.Supplier).Include(e => e.Booking).AsQueryable();
        if (filter.FromDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= filter.FromDate);
        if (filter.ToDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= filter.ToDate);

        return await query.OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseRegisterRow
            {
                ExpenseDate = e.ExpenseDate,
                ExpenseCategory = e.ExpenseCategory,
                Description = e.Description,
                SupplierName = e.Supplier != null ? e.Supplier.SupplierName : null,
                BookingNo = e.Booking != null ? e.Booking.BookingNo : null,
                Amount = e.Amount,
                PaymentMethod = e.PaymentMethod
            }).ToListAsync(cancellationToken);
    }

    public async Task<ProfitLossSummaryViewModel> GetProfitLossReportAsync(ReportFilterViewModel filter, CancellationToken cancellationToken = default)
        => await _accounts.GetProfitLossSummaryAsync(filter.FromDate, filter.ToDate, cancellationToken);

    private static void ApplyPaymentDateFilter(ref IQueryable<Models.Entities.Payment> query, ReportFilterViewModel filter)
    {
        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(p => p.PaymentDate >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var to = filter.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(p => p.PaymentDate <= to);
        }
    }
}