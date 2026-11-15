using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.CustomerOnly)]
public class CustomerPortalController : Controller
{
    private readonly ICustomerPortalService _portal;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerPortalController(ICustomerPortalService portal, UserManager<ApplicationUser> userManager)
    {
        _portal = portal;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Dashboard";
        var model = await _portal.GetDashboardAsync(_userManager.GetUserId(User)!, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Profile";
        var model = await _portal.GetProfileAsync(_userManager.GetUserId(User)!, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(CustomerProfileViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Profile";
        if (!ModelState.IsValid) return View(model);
        var (ok, message) = await _portal.UpdateProfileAsync(_userManager.GetUserId(User)!, model, cancellationToken);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> Travelers(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Travelers";
        var items = await _portal.GetTravelersAsync(_userManager.GetUserId(User)!, cancellationToken);
        return View(items);
    }

    [HttpGet]
    public IActionResult CreateTraveler()
    {
        ViewData["Title"] = "Add Traveler";
        return View("TravelerForm", new Traveler());
    }

    [HttpGet]
    public async Task<IActionResult> EditTraveler(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Traveler";
        var travelers = await _portal.GetTravelersAsync(_userManager.GetUserId(User)!, cancellationToken);
        var traveler = travelers.FirstOrDefault(t => t.TravelerId == id);
        return traveler == null ? NotFound() : View("TravelerForm", traveler);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTraveler(Traveler model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            ModelState.AddModelError(nameof(model.FullName), "Full name is required.");
            return View("TravelerForm", model);
        }

        var (ok, message) = await _portal.SaveTravelerAsync(_userManager.GetUserId(User)!, model, cancellationToken);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, message);
            return View("TravelerForm", model);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Travelers));
    }

    public async Task<IActionResult> Bookings(string? type, CancellationToken cancellationToken)
    {
        ViewData["Title"] = type switch
        {
            "Hajj" => "My Hajj Bookings",
            "Umrah" => "My Umrah Bookings",
            _ => "My Bookings"
        };
        var items = await _portal.GetBookingsAsync(_userManager.GetUserId(User)!, type, cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> BookingDetails(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Booking Details";
        var model = await _portal.GetBookingDetailsAsync(_userManager.GetUserId(User)!, id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> PrintConfirmation(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Booking Confirmation";
        var model = await _portal.GetBookingDetailsAsync(_userManager.GetUserId(User)!, id, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Payments(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Payments";
        var items = await _portal.GetPaymentsAsync(_userManager.GetUserId(User)!, cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> PrintReceipt(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Payment Receipt";
        var payments = await _portal.GetPaymentsAsync(_userManager.GetUserId(User)!, cancellationToken);
        var payment = payments.FirstOrDefault(p => p.PaymentId == id);
        return payment == null ? NotFound() : View(payment);
    }

    public async Task<IActionResult> Documents(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "My Documents";
        var items = await _portal.GetDocumentsAsync(_userManager.GetUserId(User)!, cancellationToken);
        return View(items);
    }

    public IActionResult Notifications()
    {
        ViewData["Title"] = "Notifications";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CreateBooking(int packageId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Book Package";
        var model = await _portal.GetBookingRequestAsync(_userManager.GetUserId(User)!, packageId, cancellationToken);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBooking(CustomerBookingRequestViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Book Package";
        var prepared = await _portal.GetBookingRequestAsync(_userManager.GetUserId(User)!, model.PackageId, cancellationToken);
        if (prepared == null) return NotFound();
        model.TravelerOptions = prepared.TravelerOptions;
        model.DepartureOptions = prepared.DepartureOptions;
        model.PackageName = prepared.PackageName;

        if (model.TravelerIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one traveler.");
            return View(model);
        }

        var (ok, message, bookingId) = await _portal.CreateBookingAsync(_userManager.GetUserId(User)!, model, cancellationToken);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        TempData["Success"] = "Booking created successfully.";
        return RedirectToAction(nameof(BookingDetails), new { id = bookingId });
    }
}
