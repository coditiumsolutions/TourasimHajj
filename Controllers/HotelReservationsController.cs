using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class HotelReservationsController : Controller
{
    private readonly IHotelSearchService _hotels;

    public HotelReservationsController(IHotelSearchService hotels) => _hotels = hotels;

    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Hotel Reservations";
        ViewBag.Q = q;
        ViewBag.ApiConfigured = _hotels.IsLiveSearchAvailable;
        var items = await _hotels.GetReservationsAsync(q, cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var item = await _hotels.GetReservationAsync(id, cancellationToken);
        if (item is null)
            return NotFound();

        ViewData["Title"] = "Reservation " + (item.SupplierBookingReference ?? item.HotelReservationId.ToString());
        return View(item);
    }

    public IActionResult ApiStatus()
    {
        ViewData["Title"] = "Hotel API status";
        ViewBag.ApiConfigured = _hotels.IsLiveSearchAvailable;
        return View();
    }
}
