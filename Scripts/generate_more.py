from pathlib import Path
ROOT = Path(r"D:\Dotnet\CoreCursor\Tourism Hajj")
CTRL = ROOT / "Controllers"
VIEWS = ROOT / "Views"

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.strip() + "\n", encoding="utf-8")
    print("wrote", path.relative_to(ROOT))

write(CTRL / "HajjSeasonsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class HajjSeasonsController : Controller
{
    private readonly IHajjService _service;
    public HajjSeasonsController(IHajjService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Hajj Seasons";
        var (items, total) = await _service.GetSeasonsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create() { ViewData["Title"] = "New Hajj Season"; return View(new HajjSeason { IsActive = true, HajjYear = DateTime.Now.Year }); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HajjSeason model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveSeasonAsync(model);
        TempData["Success"] = "Season saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Hajj Season";
        var model = await _service.GetSeasonAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HajjSeason model)
    {
        if (id != model.HajjSeasonId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.SaveSeasonAsync(model);
        TempData["Success"] = "Season updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Hajj Season Details";
        var model = await _service.GetSeasonAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteSeasonAsync(id);
        TempData["Success"] = "Season deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "PilgrimGroupsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class PilgrimGroupsController : Controller
{
    private readonly IHajjService _service;
    private readonly ApplicationDbContext _db;
    public PilgrimGroupsController(IHajjService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, int? seasonId, int page = 1)
    {
        ViewData["Title"] = "Pilgrim Groups";
        var (items, total) = await _service.GetGroupsAsync(q, seasonId, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Group Details";
        var model = await _service.GetGroupAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Pilgrim Group";
        await LoadLookups();
        return View(new PilgrimGroup { GroupStatus = "Open" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PilgrimGroup model)
    {
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveGroupAsync(model);
        TempData["Success"] = "Group created.";
        return RedirectToAction(nameof(Details), new { id = model.PilgrimGroupId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetGroupAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Group";
        await LoadLookups();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PilgrimGroup model)
    {
        if (id != model.PilgrimGroupId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveGroupAsync(model);
        TempData["Success"] = "Group updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(PilgrimGroupMember model)
    {
        await _service.AddGroupMemberAsync(model);
        TempData["Success"] = "Member added.";
        return RedirectToAction(nameof(Details), new { id = model.PilgrimGroupId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMember(int memberId, int groupId)
    {
        await _service.RemoveGroupMemberAsync(memberId);
        TempData["Success"] = "Member removed.";
        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    private async Task LoadLookups()
    {
        ViewBag.Seasons = new SelectList(await _db.HajjSeasons.Where(s => s.IsActive != false).ToListAsync(), "HajjSeasonId", "SeasonName");
        ViewBag.Departures = new SelectList(await _db.PackageDepartures.Include(d => d.Package).Where(d => d.Package.PackageType == "Hajj").ToListAsync(), "DepartureId", "DepartureDate");
        ViewBag.BookingTravelers = new SelectList(await _db.BookingTravelers.Include(bt => bt.Traveler).Select(bt => new { bt.BookingTravelerId, Name = bt.Traveler.FullName }).ToListAsync(), "BookingTravelerId", "Name");
    }
}
''')

write(CTRL / "VisaApplicationsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class VisaApplicationsController : Controller
{
    private readonly IUmrahService _service;
    private readonly ApplicationDbContext _db;
    public VisaApplicationsController(IUmrahService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, string? status, int page = 1)
    {
        ViewData["Title"] = "Visa Applications";
        var (items, total) = await _service.GetVisasAsync(q, status, page, 10);
        ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Visa Details";
        var model = await _service.GetVisaAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Visa Application";
        await LoadLookups();
        return View(new VisaApplication { VisaStatus = "Pending", VisaType = "Umrah" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VisaApplication model)
    {
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveVisaAsync(model);
        TempData["Success"] = "Visa application saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetVisaAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Visa Application";
        await LoadLookups();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VisaApplication model)
    {
        if (id != model.VisaApplicationId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveVisaAsync(model);
        TempData["Success"] = "Visa application updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? remarks)
    {
        await _service.UpdateVisaStatusAsync(id, status, remarks);
        TempData["Success"] = "Visa status updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadLookups()
    {
        ViewBag.BookingTravelers = new SelectList(await _db.BookingTravelers.Include(bt => bt.Traveler).Select(bt => new { bt.BookingTravelerId, Name = bt.Traveler.FullName }).ToListAsync(), "BookingTravelerId", "Name");
        ViewBag.Countries = new SelectList(await _db.Countries.Where(c => c.IsActive != false).ToListAsync(), "CountryId", "CountryName");
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false && s.SupplierType == "Visa Agent").ToListAsync(), "SupplierId", "SupplierName");
    }
}
''')

write(CTRL / "SuppliersController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ISupplierService _service;
    private readonly IMasterDataService _masters;
    public SuppliersController(ISupplierService service, IMasterDataService masters) { _service = service; _masters = masters; }

    public async Task<IActionResult> Index(string? q, string? type, int page = 1)
    {
        ViewData["Title"] = "Suppliers";
        var (items, total) = await _service.GetSuppliersAsync(q, type, page, 10);
        ViewBag.Q = q; ViewBag.Type = type; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Supplier";
        await LoadLookups();
        return View(new Supplier { IsActive = true, SupplierType = "Hotel" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier model)
    {
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveSupplierAsync(model);
        TempData["Success"] = "Supplier saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetSupplierAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Supplier";
        await LoadLookups();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier model)
    {
        if (id != model.SupplierId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveSupplierAsync(model);
        TempData["Success"] = "Supplier updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Supplier Details";
        var model = await _service.GetSupplierAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteSupplierAsync(id);
        TempData["Success"] = "Supplier deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups()
    {
        ViewBag.Countries = new SelectList(await _masters.GetActiveCountriesAsync(), "CountryId", "CountryName");
        ViewBag.Cities = new SelectList(await _masters.GetActiveCitiesAsync(), "CityId", "CityName");
    }
}
''')

write(CTRL / "HotelsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class HotelsController : Controller
{
    private readonly ISupplierService _service;
    private readonly ApplicationDbContext _db;
    public HotelsController(ISupplierService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Hotels";
        var (items, total) = await _service.GetHotelsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Hotel";
        await LoadLookups();
        return View(new Hotel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Hotel model)
    {
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveHotelAsync(model);
        TempData["Success"] = "Hotel saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetHotelAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Hotel";
        await LoadLookups();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Hotel model)
    {
        if (id != model.HotelId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(); return View(model); }
        await _service.SaveHotelAsync(model);
        TempData["Success"] = "Hotel updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Hotel Details";
        var model = await _service.GetHotelAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteHotelAsync(id);
        TempData["Success"] = "Hotel deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups()
    {
        ViewBag.Cities = new SelectList(await _db.Cities.Where(c => c.IsActive != false).ToListAsync(), "CityId", "CityName");
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName");
    }
}
''')

write(CTRL / "TransportProvidersController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class TransportProvidersController : Controller
{
    private readonly ISupplierService _service;
    private readonly ApplicationDbContext _db;
    public TransportProvidersController(ISupplierService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Transport Providers";
        var (items, total) = await _service.GetTransportsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Transport Provider";
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName");
        return View(new TransportProvider { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransportProvider model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName", model.SupplierId);
            return View(model);
        }
        await _service.SaveTransportAsync(model);
        TempData["Success"] = "Transport provider saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetTransportAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Transport Provider";
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName", model.SupplierId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TransportProvider model)
    {
        if (id != model.TransportProviderId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName", model.SupplierId);
            return View(model);
        }
        await _service.SaveTransportAsync(model);
        TempData["Success"] = "Transport provider updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Transport Details";
        var model = await _service.GetTransportAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteTransportAsync(id);
        TempData["Success"] = "Transport provider deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "PaymentsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IAccountsService _service;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public PaymentsController(IAccountsService service, ApplicationDbContext db, UserManager<ApplicationUser> users)
    { _service = service; _db = db; _users = users; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Payment Receipts";
        var (items, total) = await _service.GetPaymentsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create(int? bookingId)
    {
        ViewData["Title"] = "New Payment";
        ViewBag.Bookings = new SelectList(await _db.Bookings.Where(b => b.BookingStatus != "Cancelled").OrderByDescending(b => b.BookingId).ToListAsync(), "BookingId", "BookingNo", bookingId);
        return View(new Payment { BookingId = bookingId ?? 0, PaymentDate = DateTime.Now, PaymentMethod = "Cash", PaymentType = "Installment", CurrencyCode = "PKR" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Payment model)
    {
        var user = await _users.GetUserAsync(User);
        var (ok, message) = await _service.AddPaymentAsync(model, user?.Id);
        if (!ok)
        {
            TempData["Error"] = message;
            ViewBag.Bookings = new SelectList(await _db.Bookings.Where(b => b.BookingStatus != "Cancelled").ToListAsync(), "BookingId", "BookingNo", model.BookingId);
            return View(model);
        }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "RefundsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class RefundsController : Controller
{
    private readonly IAccountsService _service;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public RefundsController(IAccountsService service, ApplicationDbContext db, UserManager<ApplicationUser> users)
    { _service = service; _db = db; _users = users; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Refunds";
        var (items, total) = await _service.GetRefundsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create(int? bookingId)
    {
        ViewData["Title"] = "New Refund";
        ViewBag.Bookings = new SelectList(await _db.Bookings.OrderByDescending(b => b.BookingId).ToListAsync(), "BookingId", "BookingNo", bookingId);
        return View(new Refund { BookingId = bookingId ?? 0, RefundDate = DateOnly.FromDateTime(DateTime.Today), RefundStatus = "Processed", PaymentMethod = "Cash" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Refund model)
    {
        var user = await _users.GetUserAsync(User);
        var (ok, message) = await _service.AddRefundAsync(model, user?.Id);
        if (!ok)
        {
            TempData["Error"] = message;
            ViewBag.Bookings = new SelectList(await _db.Bookings.ToListAsync(), "BookingId", "BookingNo", model.BookingId);
            return View(model);
        }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "ExpensesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IAccountsService _service;
    private readonly ApplicationDbContext _db;
    public ExpensesController(IAccountsService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Expenses";
        var (items, total) = await _service.GetExpensesAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Expense";
        await LoadLookups();
        return View(new Expense { ExpenseDate = DateOnly.FromDateTime(DateTime.Today), PaymentMethod = "Cash" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense model)
    {
        try
        {
            await _service.SaveExpenseAsync(model);
            TempData["Success"] = "Expense saved.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            await LoadLookups();
            return View(model);
        }
    }

    private async Task LoadLookups()
    {
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName");
        ViewBag.Bookings = new SelectList(await _db.Bookings.OrderByDescending(b => b.BookingId).ToListAsync(), "BookingId", "BookingNo");
    }
}
''')

write(CTRL / "SupplierPaymentsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class SupplierPaymentsController : Controller
{
    private readonly IAccountsService _service;
    private readonly ApplicationDbContext _db;
    public SupplierPaymentsController(IAccountsService service, ApplicationDbContext db) { _service = service; _db = db; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Supplier Payments";
        var (items, total) = await _service.GetSupplierPaymentsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Supplier Payment";
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName");
        ViewBag.Bookings = new SelectList(await _db.Bookings.OrderByDescending(b => b.BookingId).ToListAsync(), "BookingId", "BookingNo");
        return View(new SupplierPayment { PaymentDate = DateOnly.FromDateTime(DateTime.Today), PaymentMethod = "Bank Transfer" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierPayment model)
    {
        try
        {
            await _service.SaveSupplierPaymentAsync(model);
            TempData["Success"] = "Supplier payment saved.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            ViewBag.Suppliers = new SelectList(await _db.Suppliers.Where(s => s.IsActive != false).ToListAsync(), "SupplierId", "SupplierName", model.SupplierId);
            ViewBag.Bookings = new SelectList(await _db.Bookings.ToListAsync(), "BookingId", "BookingNo", model.BookingId);
            return View(model);
        }
    }
}
''')

write(CTRL / "ReportsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    public IActionResult Index()
    {
        ViewData["Title"] = "Reports";
        return View();
    }

    private async Task<IActionResult> Run(string title, Func<ReportFilterViewModel, Task<ReportResultViewModel>> runner, ReportFilterViewModel filter)
    {
        ViewData["Title"] = title;
        var model = await runner(filter ?? new ReportFilterViewModel());
        return View("Report", model);
    }

    public Task<IActionResult> BookingRegister(ReportFilterViewModel filter) => Run("Booking Register", _reports.BookingRegisterAsync, filter);
    public Task<IActionResult> CustomerLedger(ReportFilterViewModel filter) => Run("Customer Ledger", _reports.CustomerLedgerAsync, filter);
    public Task<IActionResult> SupplierLedger(ReportFilterViewModel filter) => Run("Supplier Ledger", _reports.SupplierLedgerAsync, filter);
    public Task<IActionResult> DailyCollection(ReportFilterViewModel filter) => Run("Daily Collection", _reports.DailyCollectionAsync, filter);
    public Task<IActionResult> MonthlyCollection(ReportFilterViewModel filter) => Run("Monthly Collection", _reports.MonthlyCollectionAsync, filter);
    public Task<IActionResult> OutstandingPayments(ReportFilterViewModel filter) => Run("Outstanding Payments", _reports.OutstandingPaymentsAsync, filter);
    public Task<IActionResult> PackageSales(ReportFilterViewModel filter) => Run("Package Sales", _reports.PackageSalesAsync, filter);
    public Task<IActionResult> HajjPilgrimRegister(ReportFilterViewModel filter) => Run("Hajj Pilgrim Register", _reports.HajjPilgrimRegisterAsync, filter);
    public Task<IActionResult> UmrahPilgrimRegister(ReportFilterViewModel filter) => Run("Umrah Pilgrim Register", _reports.UmrahPilgrimRegisterAsync, filter);
    public Task<IActionResult> VisaStatus(ReportFilterViewModel filter) => Run("Visa Status Report", _reports.VisaStatusAsync, filter);
    public Task<IActionResult> PassportExpiry(ReportFilterViewModel filter) => Run("Passport Expiry Report", _reports.PassportExpiryAsync, filter);
    public Task<IActionResult> HotelRoomAllocation(ReportFilterViewModel filter) => Run("Hotel Room Allocation", _reports.HotelRoomAllocationAsync, filter);
    public Task<IActionResult> SupplierPayments(ReportFilterViewModel filter) => Run("Supplier Payments", _reports.SupplierPaymentsAsync, filter);
    public Task<IActionResult> ExpenseRegister(ReportFilterViewModel filter) => Run("Expense Register", _reports.ExpenseRegisterAsync, filter);
    public Task<IActionResult> ProfitLoss(ReportFilterViewModel filter) => Run("Profit & Loss", _reports.ProfitLossAsync, filter);
}
''')

print("more controllers done")
