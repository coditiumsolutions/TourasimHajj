using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class FlightReservationsController : Controller
{
    private readonly IFlightSearchService _flights;

    public FlightReservationsController(IFlightSearchService flights) => _flights = flights;

    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Flight Reservations";
        ViewBag.Q = q;
        ViewBag.ApiConfigured = _flights.IsLiveSearchAvailable;
        var items = await _flights.GetReservationsAsync(q, cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var item = await _flights.GetReservationAsync(id, cancellationToken);
        if (item is null)
            return NotFound();

        ViewData["Title"] = "Flight " + (item.TicketNumber ?? item.SupplierBookingReference ?? item.FlightReservationId.ToString());
        return View(item);
    }

    public IActionResult ApiStatus()
    {
        ViewData["Title"] = "Flight API status";
        ViewBag.ApiConfigured = _flights.IsLiveSearchAvailable;
        return View();
    }
}
