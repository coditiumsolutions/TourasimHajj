using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.Models;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IPublicSiteService _publicSite;

    public HomeController(IPublicSiteService publicSite) => _publicSite = publicSite;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Home";
        ViewData["MetaDescription"] = "Premium Travel, Tourism, Hajj and Umrah packages with professional service.";
        var model = await _publicSite.GetHomeAsync(cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(PackageSearchViewModel search, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Search Packages";
        ViewData["MetaDescription"] = "Search domestic, international, Hajj and Umrah travel packages.";
        search.DestinationOptions = await _publicSite.GetActiveDestinationsAsync(cancellationToken);

        var hasCriteria =
            (!string.IsNullOrWhiteSpace(search.PackageType) &&
             !string.Equals(search.PackageType, "All", StringComparison.OrdinalIgnoreCase)) ||
            search.DestinationId.HasValue ||
            search.DepartureDate.HasValue;

        ViewBag.HasSearchCriteria = hasCriteria;
        ViewBag.Results = hasCriteria
            ? await _publicSite.SearchPackagesAsync(search, cancellationToken)
            : new List<PublicPackageCardViewModel>();

        return View(search);
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms and Conditions";
        return View();
    }

    public IActionResult About()
    {
        ViewData["Title"] = "About Us";
        ViewData["MetaDescription"] = "Learn about Al-Haram Travel — trusted travel and pilgrimage specialists.";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
