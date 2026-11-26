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
public class PublicFlightsController : Controller
{
    private readonly IFlightSearchService _flights;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PublicFlightsController(
        IFlightSearchService flights,
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _flights = flights;
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("/Flights")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Flights";
        ViewData["MetaDescription"] = "Search Bahrain to Jeddah flights via Amadeus and book e-tickets.";
        var page = await _flights.SearchPageAsync(null, searched: true, cancellationToken);
        return View(page);
    }

    [HttpGet("/Flights/Search")]
    public async Task<IActionResult> Search([FromQuery] FlightSearchRequest search, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Flight search";
        var page = await _flights.SearchPageAsync(search, searched: true, cancellationToken);
        return View("Index", page);
    }

    [HttpGet("/Flights/Book")]
    public async Task<IActionResult> Book(string offerId, CancellationToken cancellationToken)
    {
        var model = await _flights.PrepareBookAsync(offerId, cancellationToken);
        if (model is null)
        {
            TempData["Error"] = "This flight offer expired. Please search again.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "Complete flight booking";
        return View(model);
    }

    [HttpPost("/Flights/Book")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(FlightBookPageViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Complete flight booking";
        model.AvailablePaymentMethods = (await _flights.PrepareBookAsync(model.OfferId, cancellationToken))
            ?.AvailablePaymentMethods ?? model.AvailablePaymentMethods;

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

        var (ok, reservation, error) = await _flights.BookAsync(model, customerId, cancellationToken);
        if (!ok || reservation is null)
        {
            model.ErrorMessage = error ?? "Booking failed.";
            return View(model);
        }

        return RedirectToAction(nameof(Confirmation), new { id = reservation.FlightReservationId });
    }

    [HttpGet("/Flights/Confirmation/{id:int}")]
    public async Task<IActionResult> Confirmation(int id, CancellationToken cancellationToken)
    {
        var reservation = await _flights.GetReservationAsync(id, cancellationToken);
        if (reservation is null)
            return NotFound();

        ViewData["Title"] = "Flight confirmation";
        return View(reservation);
    }
}
