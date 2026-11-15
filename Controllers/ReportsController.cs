using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class ReportsController : Controller
{
    private readonly IReportService _reports;
    private readonly ApplicationDbContext _db;

    public ReportsController(IReportService reports, ApplicationDbContext db)
    {
        _reports = reports;
        _db = db;
    }

    public IActionResult Index() => View(_reports.GetReportMenu());

    private ReportFilterViewModel DefaultFilter(string title, string action)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return new ReportFilterViewModel
        {
            ReportTitle = title,
            ReportAction = action,
            FromDate = today.AddMonths(-1),
            ToDate = today
        };
    }

    private async Task PopulateReportLookupsAsync(ReportFilterViewModel filter, bool customers, bool suppliers, CancellationToken cancellationToken)
    {
        if (customers)
        {
            ViewBag.Customers = new SelectList(
                await _db.Customers.AsNoTracking().OrderBy(c => c.CustomerName).Take(500).ToListAsync(cancellationToken),
                nameof(Models.Entities.Customer.CustomerId),
                nameof(Models.Entities.Customer.CustomerName),
                filter.CustomerId);
        }

        if (suppliers)
        {
            ViewBag.Suppliers = new SelectList(
                await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken),
                nameof(Models.Entities.Supplier.SupplierId),
                nameof(Models.Entities.Supplier.SupplierName),
                filter.SupplierId);
        }
    }

    public IActionResult BookingRegister() => View(DefaultFilter("Booking Register", nameof(BookingRegister)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BookingRegister(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Booking Register";
        ViewBag.Rows = await _reports.GetBookingRegisterAsync(filter, cancellationToken);
        return View(filter);
    }

    public async Task<IActionResult> CustomerLedger(CancellationToken cancellationToken)
    {
        var filter = DefaultFilter("Customer Ledger", nameof(CustomerLedger));
        await PopulateReportLookupsAsync(filter, customers: true, suppliers: false, cancellationToken);
        return View(filter);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomerLedger(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Customer Ledger";
        ViewBag.Rows = await _reports.GetCustomerLedgerReportAsync(filter, cancellationToken);
        await PopulateReportLookupsAsync(filter, customers: true, suppliers: false, cancellationToken);
        return View(filter);
    }

    public async Task<IActionResult> SupplierLedger(CancellationToken cancellationToken)
    {
        var filter = DefaultFilter("Supplier Ledger", nameof(SupplierLedger));
        await PopulateReportLookupsAsync(filter, customers: false, suppliers: true, cancellationToken);
        return View(filter);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SupplierLedger(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Supplier Ledger";
        ViewBag.Rows = await _reports.GetSupplierLedgerReportAsync(filter, cancellationToken);
        await PopulateReportLookupsAsync(filter, customers: false, suppliers: true, cancellationToken);
        return View(filter);
    }

    public IActionResult DailyCollection() => View(DefaultFilter("Daily Collection", nameof(DailyCollection)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DailyCollection(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Daily Collection";
        ViewBag.Rows = await _reports.GetDailyCollectionAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult MonthlyCollection() => View(DefaultFilter("Monthly Collection", nameof(MonthlyCollection)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MonthlyCollection(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Monthly Collection";
        ViewBag.Rows = await _reports.GetMonthlyCollectionAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult OutstandingPayments() => View(DefaultFilter("Outstanding Payments", nameof(OutstandingPayments)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> OutstandingPayments(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Outstanding Payments";
        ViewBag.Rows = await _reports.GetOutstandingPaymentsAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult PackageSales() => View(DefaultFilter("Package Sales", nameof(PackageSales)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PackageSales(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Package Sales";
        ViewBag.Rows = await _reports.GetPackageSalesAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult HajjPilgrimRegister() => View(DefaultFilter("Hajj Pilgrim Register", nameof(HajjPilgrimRegister)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> HajjPilgrimRegister(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Hajj Pilgrim Register";
        ViewBag.Rows = await _reports.GetHajjPilgrimRegisterAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult UmrahPilgrimRegister() => View(DefaultFilter("Umrah Pilgrim Register", nameof(UmrahPilgrimRegister)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UmrahPilgrimRegister(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Umrah Pilgrim Register";
        ViewBag.Rows = await _reports.GetUmrahPilgrimRegisterAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult VisaStatus() => View(DefaultFilter("Visa Status", nameof(VisaStatus)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> VisaStatus(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Visa Status";
        ViewBag.Rows = await _reports.GetVisaStatusAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult PassportExpiry()
    {
        var filter = DefaultFilter("Passport Expiry", nameof(PassportExpiry));
        filter.PassportExpiryDays = 90;
        return View(filter);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PassportExpiry(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Passport Expiry";
        ViewBag.Rows = await _reports.GetPassportExpiryAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult HotelRoomAllocation() => View(DefaultFilter("Hotel Room Allocation", nameof(HotelRoomAllocation)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> HotelRoomAllocation(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Hotel Room Allocation";
        ViewBag.Rows = await _reports.GetHotelRoomAllocationAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult SupplierPayments() => View(DefaultFilter("Supplier Payments", nameof(SupplierPayments)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SupplierPayments(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Supplier Payments";
        ViewBag.Rows = await _reports.GetSupplierPaymentsReportAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult ExpenseRegister() => View(DefaultFilter("Expense Register", nameof(ExpenseRegister)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExpenseRegister(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Expense Register";
        ViewBag.Rows = await _reports.GetExpenseRegisterAsync(filter, cancellationToken);
        return View(filter);
    }

    public IActionResult ProfitLoss() => View(DefaultFilter("Profit & Loss", nameof(ProfitLoss)));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfitLoss(ReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.ReportTitle = "Profit & Loss";
        ViewBag.Summary = await _reports.GetProfitLossReportAsync(filter, cancellationToken);
        return View(filter);
    }
}