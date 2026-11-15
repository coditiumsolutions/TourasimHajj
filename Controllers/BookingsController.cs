using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
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
