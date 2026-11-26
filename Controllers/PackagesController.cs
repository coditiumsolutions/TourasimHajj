using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class PackagesController : Controller
{
    private readonly IPublicSiteService _publicSite;
    private readonly ApplicationDbContext _db;

    public PackagesController(IPublicSiteService publicSite, ApplicationDbContext db)
    {
        _publicSite = publicSite;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _publicSite.GetPackageDetailsAsync(id, cancellationToken);
        if (model == null) return NotFound();

        ViewData["Title"] = model.Package.PackageName;
        ViewData["MetaDescription"] = $"{model.Package.PackageName} — {model.Package.PackageType} package details, itinerary and departures.";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Book(int id, CancellationToken cancellationToken)
    {
        var packageType = await _db.TravelPackages.AsNoTracking()
            .Where(p => p.PackageId == id)
            .Select(p => p.PackageType)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.Equals(packageType, "Hajj", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Info"] = "Hajj packages are enquiry-only (Ministry / Nusuk campaigns). Please register your interest.";
            return RedirectToAction("Index", "Contact", new { subject = "Hajj interest" });
        }

        if (User.Identity?.IsAuthenticated != true || !User.IsInRole(AppRoles.Customer))
        {
            var returnUrl = Url.Action("Book", "Packages", new { id }) ?? $"/Packages/Book/{id}";
            return RedirectToAction("Login", "CustomerAccount", new { returnUrl });
        }

        return RedirectToAction("CreateBooking", "CustomerPortal", new { packageId = id });
    }
}
