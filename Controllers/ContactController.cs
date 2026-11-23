using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class ContactController : Controller
{
    private readonly IPublicSiteService _publicSite;

    public ContactController(IPublicSiteService publicSite) => _publicSite = publicSite;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Contact Us";
        ViewData["MetaDescription"] = "Contact Al-Haram Travel for tourism, Hajj and Umrah travel assistance.";
        return View(new ContactFormViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Contact Us";
        if (!ModelState.IsValid) return View(model);

        await _publicSite.SubmitContactAsync(model, cancellationToken);
        TempData["Success"] = "Thank you. Your message has been received.";
        return RedirectToAction(nameof(Index));
    }
}
