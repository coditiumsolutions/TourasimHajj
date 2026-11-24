using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class TravelersController : Controller
{
    private readonly ITravelerService _service;
    private readonly IMasterDataService _masters;
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public TravelersController(
        ITravelerService service,
        IMasterDataService masters,
        ApplicationDbContext db,
        IWebHostEnvironment env)
    {
        _service = service;
        _masters = masters;
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index(string? q, int? customerId, int page = 1)
    {
        ViewData["Title"] = "Travelers";
        var (items, total) = await _service.GetPagedAsync(q, customerId, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> PassportAlerts()
    {
        ViewData["Title"] = "Passport Management";
        return View(await _service.GetPassportAlertsAsync());
    }

    public async Task<IActionResult> Documents(int? travelerId, string? q, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Traveler Documents";
        var model = await _service.GetDocumentsPageAsync(travelerId, q, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(TravelerDocumentInputModel model, IFormFile? documentFile, CancellationToken cancellationToken)
    {
        if (model.TravelerId <= 0)
            ModelState.AddModelError(nameof(model.TravelerId), "Select a traveler.");

        string? filePath = null;
        try
        {
            filePath = await PackageImageStorage.SaveTravelerDocumentAsync(_env, documentFile, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        if (documentFile == null || documentFile.Length == 0)
            ModelState.AddModelError(string.Empty, "Please choose a file to upload.");

        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(filePath))
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault()
                ?? "Document was not uploaded.";
            return RedirectToAction(nameof(Documents), new { travelerId = model.TravelerId });
        }

        await _service.AddDocumentAsync(new TravelerDocument
        {
            TravelerId = model.TravelerId,
            DocumentType = model.DocumentType.Trim(),
            DocumentNumber = string.IsNullOrWhiteSpace(model.DocumentNumber) ? null : model.DocumentNumber.Trim(),
            IssueDate = model.IssueDate,
            ExpiryDate = model.ExpiryDate,
            Remarks = model.Remarks,
            FilePath = filePath
        });

        TempData["Success"] = "Document uploaded.";
        return RedirectToAction(nameof(Documents), new { travelerId = model.TravelerId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id, int travelerId, CancellationToken cancellationToken)
    {
        await _service.DeleteDocumentAsync(id, cancellationToken);
        TempData["Success"] = "Document removed.";
        return RedirectToAction(nameof(Documents), new { travelerId });
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Traveler Details";
        var model = await _service.GetByIdAsync(id);
        return model == null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create(int? customerId)
    {
        ViewData["Title"] = "New Traveler";
        await LoadLookups(customerId);
        return View(new Traveler { CustomerId = customerId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Traveler model)
    {
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model == null) return NotFound();
        ViewData["Title"] = "Edit Traveler";
        await LoadLookups(model.CustomerId);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Traveler model)
    {
        if (id != model.TravelerId) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(model.CustomerId); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Traveler updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Traveler deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups(int? customerId)
    {
        ViewBag.Customers = new SelectList(await _db.Customers.Where(c => c.IsActive != false).OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", customerId);
        ViewBag.Countries = new SelectList(await _masters.GetActiveCountriesListAsync(), nameof(Country.CountryId), nameof(Country.CountryName));
    }
}
