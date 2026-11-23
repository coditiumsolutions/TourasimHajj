using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class PublicGalleryController : Controller
{
    private readonly IPublicSiteService _publicSite;

    public PublicGalleryController(IPublicSiteService publicSite) => _publicSite = publicSite;

    [HttpGet("/Gallery")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Gallery";
        ViewData["MetaDescription"] = "Photo gallery of pilgrimage and travel experiences.";
        var items = await _publicSite.GetPublicGalleryAsync(cancellationToken);
        return View(items);
    }
}
