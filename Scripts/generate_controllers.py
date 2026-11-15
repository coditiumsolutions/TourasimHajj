from pathlib import Path

ROOT = Path(r"D:\Dotnet\CoreCursor\Tourism Hajj")
CTRL = ROOT / "Controllers"
VIEWS = ROOT / "Views"

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.strip() + "\n", encoding="utf-8")
    print("wrote", path.relative_to(ROOT))

# ---- Controllers ----

write(CTRL / "CountriesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class CountriesController : Controller
{
    private readonly IMasterDataService _service;
    public CountriesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Countries";
        var (items, total) = await _service.GetCountriesAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create() { ViewData["Title"] = "New Country"; return View(new Country { IsActive = true }); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Country model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveCountryAsync(model);
        TempData["Success"] = "Country saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Country";
        var model = await _service.GetCountryAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Country model)
    {
        if (id != model.CountryId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.SaveCountryAsync(model);
        TempData["Success"] = "Country updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Country Details";
        var model = await _service.GetCountryAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteCountryAsync(id);
        TempData["Success"] = "Country deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "CitiesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class CitiesController : Controller
{
    private readonly IMasterDataService _service;
    public CitiesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int? countryId, int page = 1)
    {
        ViewData["Title"] = "Cities";
        var (items, total) = await _service.GetCitiesAsync(q, countryId, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesAsync(), "CountryId", "CountryName", countryId);
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New City";
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesAsync(), "CountryId", "CountryName");
        return View(new City { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(City model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = new SelectList(await _service.GetActiveCountriesAsync(), "CountryId", "CountryName", model.CountryId);
            return View(model);
        }
        await _service.SaveCityAsync(model);
        TempData["Success"] = "City saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetCityAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit City";
        ViewBag.Countries = new SelectList(await _service.GetActiveCountriesAsync(), "CountryId", "CountryName", model.CountryId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, City model)
    {
        if (id != model.CityId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = new SelectList(await _service.GetActiveCountriesAsync(), "CountryId", "CountryName", model.CountryId);
            return View(model);
        }
        await _service.SaveCityAsync(model);
        TempData["Success"] = "City updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "City Details";
        var model = await _service.GetCityAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteCityAsync(id);
        TempData["Success"] = "City deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "DestinationsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class DestinationsController : Controller
{
    private readonly IMasterDataService _service;
    public DestinationsController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Destinations";
        var (items, total) = await _service.GetDestinationsAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Destination";
        ViewBag.Cities = new SelectList(await _service.GetActiveCitiesAsync(), "CityId", "CityName");
        return View(new Destination { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Destination model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cities = new SelectList(await _service.GetActiveCitiesAsync(), "CityId", "CityName", model.CityId);
            return View(model);
        }
        await _service.SaveDestinationAsync(model);
        TempData["Success"] = "Destination saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetDestinationAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Destination";
        ViewBag.Cities = new SelectList(await _service.GetActiveCitiesAsync(), "CityId", "CityName", model.CityId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Destination model)
    {
        if (id != model.DestinationId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Cities = new SelectList(await _service.GetActiveCitiesAsync(), "CityId", "CityName", model.CityId);
            return View(model);
        }
        await _service.SaveDestinationAsync(model);
        TempData["Success"] = "Destination updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Destination Details";
        var model = await _service.GetDestinationAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteDestinationAsync(id);
        TempData["Success"] = "Destination deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "CatalogServicesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class ServicesController : Controller
{
    private readonly IMasterDataService _service;
    public ServicesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Services";
        var (items, total) = await _service.GetServicesAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create() { ViewData["Title"] = "New Service"; return View(new Service { IsActive = true }); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Service model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveServiceAsync(model);
        TempData["Success"] = "Service saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Service";
        var model = await _service.GetServiceAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Service model)
    {
        if (id != model.ServiceId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.SaveServiceAsync(model);
        TempData["Success"] = "Service updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Service Details";
        var model = await _service.GetServiceAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteServiceAsync(id);
        TempData["Success"] = "Service deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

# Rename file to ServicesController.cs
(CTRL / "CatalogServicesController.cs").replace(CTRL / "ServicesController.cs")

write(CTRL / "EmployeesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IMasterDataService _service;
    public EmployeesController(IMasterDataService service) => _service = service;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Employees";
        var (items, total) = await _service.GetEmployeesAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create() { ViewData["Title"] = "New Employee"; return View(new Employee { IsActive = true }); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveEmployeeAsync(model);
        TempData["Success"] = "Employee saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Employee";
        var model = await _service.GetEmployeeAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee model)
    {
        if (id != model.EmployeeId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.SaveEmployeeAsync(model);
        TempData["Success"] = "Employee updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Employee Details";
        var model = await _service.GetEmployeeAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteEmployeeAsync(id);
        TempData["Success"] = "Employee deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "TravelersController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class TravelersController : Controller
{
    private readonly ITravelerService _service;
    private readonly IMasterDataService _masters;
    private readonly ApplicationDbContext _db;

    public TravelersController(ITravelerService service, IMasterDataService masters, ApplicationDbContext db)
    {
        _service = service; _masters = masters; _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? customerId, int page = 1)
    {
        ViewData["Title"] = "Travelers";
        var (items, total) = await _service.GetPagedAsync(q, customerId, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> PassportAlerts()
    {
        ViewData["Title"] = "Passport Management";
        return View(await _service.GetPassportAlertsAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Traveler Details";
        var model = await _service.GetByIdAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create(int? customerId)
    {
        ViewData["Title"] = "New Traveler";
        await LoadLookups(customerId);
        return View(new Traveler { CustomerId = customerId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Traveler model)
    {
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Traveler";
        await LoadLookups(model.CustomerId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Traveler model)
    {
        if (id != model.TravelerId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Traveler deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups(int? customerId)
    {
        ViewBag.Customers = new SelectList(await _db.Customers.Where(c => c.IsActive != false).OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", customerId);
        ViewBag.Countries = new SelectList(await _masters.GetActiveCountriesAsync(), "CountryId", "CountryName");
    }
}
''')

write(CTRL / "TravelPackagesController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class TravelPackagesController : Controller
{
    private readonly IPackageService _service;
    public TravelPackagesController(IPackageService service) => _service = service;

    public async Task<IActionResult> Index(string? q, string? type, int page = 1)
    {
        ViewData["Title"] = string.IsNullOrWhiteSpace(type) ? "Travel Packages" : $"{type} Packages";
        var (items, total) = await _service.GetPagedAsync(q, type, page, 10);
        ViewBag.Q = q; ViewBag.Type = type; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "New Package";
        return View(new TravelPackage { IsActive = true, CurrencyCode = "PKR", PackageType = "Tourism" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.SaveAsync(model);
        TempData["Success"] = "Package created.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Package";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBasic(TravelPackage model)
    {
        await _service.SaveAsync(model);
        TempData["Success"] = "Package updated.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDeparture(PackageDeparture model)
    {
        if (model.AvailableSeats == null) model.AvailableSeats = model.TotalSeats;
        await _service.SaveDepartureAsync(model);
        TempData["Success"] = "Departure saved.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItinerary(PackageItinerary model)
    {
        await _service.SaveItineraryAsync(model);
        TempData["Success"] = "Itinerary day saved.";
        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Package Details";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteAsync(id);
        TempData["Success"] = "Package deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
''')

write(CTRL / "BookingsController.cs", r'''
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingService _service;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public BookingsController(IBookingService service, ApplicationDbContext db, UserManager<ApplicationUser> users)
    {
        _service = service; _db = db; _users = users;
    }

    public async Task<IActionResult> Index(string? q, string? type, string? status, int page = 1)
    {
        ViewData["Title"] = "Bookings";
        var (items, total) = await _service.GetPagedAsync(q, type, status, page, 10);
        ViewBag.Q = q; ViewBag.Type = type; ViewBag.Status = status; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Booking Details";
        var model = await _service.GetDetailsAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> PrintConfirmation(int id)
    {
        ViewData["Title"] = "Booking Confirmation";
        var model = await _service.GetDetailsAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Booking";
        await LoadLookups();
        return View(new BookingCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateViewModel model)
    {
        var user = await _users.GetUserAsync(User);
        var (ok, message, bookingId) = await _service.CreateBookingAsync(model, user?.Id);
        if (!ok)
        {
            TempData["Error"] = message;
            await LoadLookups(model);
            return View(model);
        }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Details), new { id = bookingId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _users.GetUserAsync(User);
        var (ok, message) = await _service.CancelAsync(id, user?.Id);
        TempData[ok ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadLookups(BookingCreateViewModel? model = null)
    {
        ViewBag.Customers = new SelectList(await _db.Customers.Where(c => c.IsActive != false).OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", model?.CustomerId);
        ViewBag.Packages = new SelectList(await _db.TravelPackages.Where(p => p.IsActive != false).OrderBy(p => p.PackageName).ToListAsync(), "PackageId", "PackageName", model?.PackageId);
        ViewBag.Departures = new SelectList(await _db.PackageDepartures.Include(d => d.Package).OrderBy(d => d.DepartureDate).Select(d => new { d.DepartureId, Name = d.Package.PackageName + " - " + d.DepartureDate }).ToListAsync(), "DepartureId", "Name", model?.DepartureId);
        ViewBag.Travelers = new MultiSelectList(await _db.Travelers.OrderBy(t => t.FullName).ToListAsync(), "TravelerId", "FullName", model?.TravelerIds);
    }
}
''')

print("core controllers done")
