using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class TravelPackagesController : Controller
{
    private readonly IPackageService _service;
    private readonly IWebHostEnvironment _env;

    public TravelPackagesController(IPackageService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    public async Task<IActionResult> Index(string? q, string? type, int page = 1)
    {
        ViewData["Title"] = string.IsNullOrWhiteSpace(type) ? "Travel Packages" : $"{type} Packages";
        var (items, total) = await _service.GetPagedAsync(q, type, page, 10);
        ViewBag.Q = q; ViewBag.Type = type; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "New Package";
        return View(new TravelPackage { IsActive = true, CurrencyCode = "PKR", PackageType = "Tourism" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage model, IFormFile? coverImage, CancellationToken cancellationToken)
    {
        ValidatePackage(model);

        try
        {
            var uploaded = await PackageImageStorage.SaveCoverAsync(_env, coverImage, cancellationToken);
            if (!string.IsNullOrWhiteSpace(uploaded))
                model.CoverImageUrl = uploaded;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.CoverImageUrl), ex.Message);
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = BuildFailureMessage("Package was not saved.", ModelState);
            return View(model);
        }

        try
        {
            await _service.SaveAsync(model);
            TempData["Success"] = $"Package saved successfully. Code: {model.PackageCode}, Name: {model.PackageName}.";
            return RedirectToAction(nameof(Edit), new { id = model.PackageId });
        }
        catch (Exception ex)
        {
            var reason = DescribeSaveException(ex);
            ModelState.AddModelError(string.Empty, reason);
            TempData["Error"] = $"Package was not saved. {reason}";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Package";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBasic([Bind(Prefix = "Package")] TravelPackage model, IFormFile? coverImage, CancellationToken cancellationToken)
    {
        ValidatePackage(model, "Package.");

        try
        {
            var uploaded = await PackageImageStorage.SaveCoverAsync(_env, coverImage, cancellationToken);
            if (!string.IsNullOrWhiteSpace(uploaded))
                model.CoverImageUrl = uploaded;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = $"Package was not saved. {ex.Message}";
            return RedirectToAction(nameof(Edit), new { id = model.PackageId });
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = BuildFailureMessage("Package was not saved.", ModelState);
            return RedirectToAction(nameof(Edit), new { id = model.PackageId });
        }

        try
        {
            await _service.SaveAsync(model);
            TempData["Success"] = $"Package updated successfully. Code: {model.PackageCode}, Name: {model.PackageName}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Package was not saved. {DescribeSaveException(ex)}";
            return RedirectToAction(nameof(Edit), new { id = model.PackageId });
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDeparture(PackageDeparture model)
    {
        try
        {
            if (model.AvailableSeats == null) model.AvailableSeats = model.TotalSeats;
            await _service.SaveDepartureAsync(model);
            TempData["Success"] = "Departure saved successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Departure was not saved. {DescribeSaveException(ex)}";
        }

        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItinerary(PackageItinerary model)
    {
        try
        {
            await _service.SaveItineraryAsync(model);
            TempData["Success"] = "Itinerary day saved successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Itinerary was not saved. {DescribeSaveException(ex)}";
        }

        return RedirectToAction(nameof(Edit), new { id = model.PackageId });
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Package Details";
        var model = await _service.GetEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.SoftDeleteAsync(id);
            TempData["Success"] = "Package deactivated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Package could not be deactivated. {DescribeSaveException(ex)}";
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidatePackage(TravelPackage model, string prefix = "")
    {
        if (string.IsNullOrWhiteSpace(model.PackageCode))
            ModelState.AddModelError($"{prefix}{nameof(model.PackageCode)}", "Package code is required.");
        if (string.IsNullOrWhiteSpace(model.PackageName))
            ModelState.AddModelError($"{prefix}{nameof(model.PackageName)}", "Package name is required.");
        if (string.IsNullOrWhiteSpace(model.PackageType))
            ModelState.AddModelError($"{prefix}{nameof(model.PackageType)}", "Package type is required.");
        else
        {
            var allowedTypes = new[] { "Tourism", "Domestic", "International", "Hajj", "Umrah", "Ziarat" };
            if (!allowedTypes.Contains(model.PackageType, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError($"{prefix}{nameof(model.PackageType)}",
                    "Package type is not allowed. Use one of: Tourism, Domestic, International, Hajj, Umrah, Ziarat.");
            }
        }
        if (model.DurationDays is < 1)
            ModelState.AddModelError($"{prefix}{nameof(model.DurationDays)}", "Days must be at least 1.");
        if (model.DurationNights is < 0)
            ModelState.AddModelError($"{prefix}{nameof(model.DurationNights)}", "Nights cannot be negative.");
        if (model.AdultPrice is < 0)
            ModelState.AddModelError($"{prefix}{nameof(model.AdultPrice)}", "Adult price cannot be negative.");
        if (model.ChildPrice is < 0)
            ModelState.AddModelError($"{prefix}{nameof(model.ChildPrice)}", "Child price cannot be negative.");
        if (model.InfantPrice is < 0)
            ModelState.AddModelError($"{prefix}{nameof(model.InfantPrice)}", "Infant price cannot be negative.");
        if (model.MinimumPersons.HasValue && model.MaximumPersons.HasValue &&
            model.MinimumPersons > model.MaximumPersons)
        {
            ModelState.AddModelError($"{prefix}{nameof(model.MaximumPersons)}",
                "Maximum persons must be greater than or equal to minimum persons.");
        }
    }

    private static string BuildFailureMessage(string prefix, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
    {
        var reasons = modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .Take(5)
            .ToList();

        return reasons.Count == 0
            ? $"{prefix} Please check the form and try again."
            : $"{prefix} {string.Join(" ", reasons!)}";
    }

    private static string DescribeSaveException(Exception ex)
    {
        if (ex is DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sql)
            {
                if (sql.Number is 2601 or 2627)
                    return "A package with this code already exists. Please use a unique package code.";

                if (sql.Number == 547)
                    return DescribeConstraintConflict(sql.Message);

                return $"Database error: {sql.Message}";
            }

            return dbEx.InnerException?.Message ?? dbEx.Message;
        }

        return ex.InnerException?.Message ?? ex.Message;
    }

    private static string DescribeConstraintConflict(string sqlMessage)
    {
        if (sqlMessage.Contains("CK_TravelPackages_Type", StringComparison.OrdinalIgnoreCase) ||
            (sqlMessage.Contains("CHECK constraint", StringComparison.OrdinalIgnoreCase) &&
             sqlMessage.Contains("PackageType", StringComparison.OrdinalIgnoreCase)))
        {
            return "Package type is not allowed. Use one of: Tourism, Domestic, International, Hajj, Umrah, Ziarat.";
        }

        if (sqlMessage.Contains("CHECK constraint", StringComparison.OrdinalIgnoreCase))
            return $"Save blocked by a database check rule. {ExtractConstraintDetail(sqlMessage)}";

        if (sqlMessage.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
            sqlMessage.Contains("REFERENCE constraint", StringComparison.OrdinalIgnoreCase))
            return $"Save blocked by a linked record rule. {ExtractConstraintDetail(sqlMessage)}";

        return $"Save blocked by a database rule. {ExtractConstraintDetail(sqlMessage)}";
    }

    private static string ExtractConstraintDetail(string sqlMessage)
    {
        // Keep the useful part of SQL Server text, without dumping the full stack.
        var text = sqlMessage.Replace("\r", " ").Replace("\n", " ").Trim();
        if (text.Length > 220)
            text = text[..220].Trim() + "...";
        return text;
    }
}
