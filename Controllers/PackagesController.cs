using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class PackagesController : Controller
{
    private readonly IPublicSiteService _publicSite;

    public PackagesController(IPublicSiteService publicSite) => _publicSite = publicSite;

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
    public IActionResult Book(int id)
    {
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole(AppRoles.Customer))
        {
            var returnUrl = Url.Action("Book", "Packages", new { id }) ?? $"/Packages/Book/{id}";
            return RedirectToAction("Login", "CustomerAccount", new { returnUrl });
        }

        return RedirectToAction("CreateBooking", "CustomerPortal", new { packageId = id });
    }
}
