using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class PublicHotelsController : Controller
{
    private readonly IPublicSiteService _publicSite;

    public PublicHotelsController(IPublicSiteService publicSite) => _publicSite = publicSite;

    [HttpGet("/Hotels")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Hotels";
        ViewData["MetaDescription"] = "Browse partner hotels for Hajj, Umrah and tourism packages.";
        var hotels = await _publicSite.GetPublicHotelsAsync(cancellationToken);
        return View(hotels);
    }
}
