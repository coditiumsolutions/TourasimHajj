using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class PublicHotelsController : Controller
{
    private readonly IHotelSearchService _hotels;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PublicHotelsController(
        IHotelSearchService hotels,
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _hotels = hotels;
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("/Hotels")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Hotels";
        ViewData["MetaDescription"] = "Search live Makkah and Madinah hotels or browse curated hotel packages.";
        // Load default-date availability immediately so the client page shows API-style results.
        var page = await _hotels.SearchPageAsync(null, searched: true, cancellationToken);
        return View(page);
    }

    [HttpGet("/Hotels/Search")]
    public async Task<IActionResult> Search(
        [FromQuery] HotelSearchRequest search,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Hotel search";
        ViewData["MetaDescription"] = "Live hotel availability for Makkah and Madinah.";
        var page = await _hotels.SearchPageAsync(search, searched: true, cancellationToken);
        return View("Index", page);
    }

    [HttpGet("/Hotels/Details/{hotelCode}")]
    public async Task<IActionResult> Details(
        string hotelCode,
        [FromQuery] HotelSearchRequest search,
        CancellationToken cancellationToken)
    {
        var detail = await _hotels.GetDetailAsync(hotelCode, search, cancellationToken);
        if (detail is null)
        {
            TempData["Error"] = "Hotel not found for those dates. Please search again.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = detail.Hotel.HotelName;
        return View(detail);
    }

    [HttpGet("/Hotels/Book")]
    public async Task<IActionResult> Book(string rateKey, string? destination, CancellationToken cancellationToken)
    {
        var model = await _hotels.PrepareBookAsync(rateKey, cancellationToken);
        if (model is null)
        {
            TempData["Error"] = "This rate expired or is unavailable. Please search again.";
            return RedirectToAction(nameof(Index));
        }

        if (!string.IsNullOrWhiteSpace(destination))
            model.Destination = destination;

        ViewData["Title"] = "Complete hotel booking";
        return View(model);
    }

    [HttpPost("/Hotels/Book")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(HotelBookPageViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Complete hotel booking";
        if (!ModelState.IsValid)
            return View(model);

        int? customerId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                customerId = await _db.Customers.AsNoTracking()
                    .Where(c => c.AspNetUserId == user.Id)
                    .Select(c => (int?)c.CustomerId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        var (ok, reservation, error) = await _hotels.BookAsync(model, customerId, cancellationToken);
        if (!ok || reservation is null)
        {
            model.ErrorMessage = error ?? "Booking failed.";
            return View(model);
        }

        return RedirectToAction(nameof(Confirmation), new { id = reservation.HotelReservationId });
    }

    [HttpGet("/Hotels/Confirmation/{id:int}")]
    public async Task<IActionResult> Confirmation(int id, CancellationToken cancellationToken)
    {
        var reservation = await _hotels.GetReservationAsync(id, cancellationToken);
        if (reservation is null)
            return NotFound();

        ViewData["Title"] = "Booking confirmation";
        return View(reservation);
    }
}
