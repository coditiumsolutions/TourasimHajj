using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class HotelPackagesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public HotelPackagesController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = includeInactive ? "Inactive Hotel Packages" : "Hotel Packages";
        var query = _db.HotelPackages.AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(h => h.IsActive);

        var items = await query
            .OrderBy(h => h.SortOrder)
            .ThenByDescending(h => h.IsFeatured)
            .ThenBy(h => h.HotelName)
            .ToListAsync(cancellationToken);

        ViewBag.IncludeInactive = includeInactive;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Add Hotel Package";
        return View(new HotelPackageInputModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelPackageInputModel model, IFormFile? coverImage, CancellationToken cancellationToken)
    {
        await ApplyCoverImageAsync(model, coverImage, cancellationToken);
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Add Hotel Package";
            return View(model);
        }

        _db.HotelPackages.Add(MapToEntity(model, new HotelPackage { CreatedDate = DateTime.Now }));
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Hotel package created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.HotelPackages.FindAsync([id], cancellationToken);
        if (entity is null) return NotFound();

        ViewData["Title"] = "Edit Hotel Package";
        return View(MapToInput(entity));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HotelPackageInputModel model, IFormFile? coverImage, CancellationToken cancellationToken)
    {
        var entity = await _db.HotelPackages.FindAsync([id], cancellationToken);
        if (entity is null) return NotFound();

        model.HotelPackageId = id;
        await ApplyCoverImageAsync(model, coverImage, cancellationToken);
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Hotel Package";
            return View(model);
        }

        MapToEntity(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Hotel package updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.HotelPackages.AsNoTracking()
            .FirstOrDefaultAsync(h => h.HotelPackageId == id, cancellationToken);
        if (entity is null) return NotFound();
        ViewData["Title"] = "Hotel Package Details";
        return View(entity);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.HotelPackages.AsNoTracking()
            .FirstOrDefaultAsync(h => h.HotelPackageId == id, cancellationToken);
        if (entity is null) return NotFound();
        ViewData["Title"] = "Deactivate Hotel Package";
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.HotelPackages.FindAsync([id], cancellationToken);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Hotel package deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ApplyCoverImageAsync(HotelPackageInputModel model, IFormFile? coverImage, CancellationToken cancellationToken)
    {
        try
        {
            var uploaded = await PackageImageStorage.SaveHotelAsync(_env, coverImage, cancellationToken);
            if (!string.IsNullOrWhiteSpace(uploaded))
                model.CoverImageUrl = uploaded;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.CoverImageUrl), ex.Message);
        }
    }

    private static HotelPackageInputModel MapToInput(HotelPackage entity) => new()
    {
        HotelPackageId = entity.HotelPackageId,
        HotelName = entity.HotelName,
        Description = entity.Description,
        CityName = entity.CityName,
        StarRating = entity.StarRating,
        Address = entity.Address,
        DistanceFromHaramKm = entity.DistanceFromHaramKm,
        DistanceFromNabawiKm = entity.DistanceFromNabawiKm,
        ContactNo = entity.ContactNo,
        CoverImageUrl = entity.CoverImageUrl,
        PricePerNight = entity.PricePerNight,
        CurrencyCode = entity.CurrencyCode ?? "PKR",
        DurationNights = entity.DurationNights,
        SortOrder = entity.SortOrder,
        IsFeatured = entity.IsFeatured,
        IsActive = entity.IsActive
    };

    private static HotelPackage MapToEntity(HotelPackageInputModel model, HotelPackage entity)
    {
        entity.HotelName = model.HotelName.Trim();
        entity.Description = model.Description;
        entity.CityName = string.IsNullOrWhiteSpace(model.CityName) ? null : model.CityName.Trim();
        entity.StarRating = model.StarRating;
        entity.Address = model.Address;
        entity.DistanceFromHaramKm = model.DistanceFromHaramKm;
        entity.DistanceFromNabawiKm = model.DistanceFromNabawiKm;
        entity.ContactNo = model.ContactNo;
        entity.CoverImageUrl = model.CoverImageUrl;
        entity.PricePerNight = model.PricePerNight;
        entity.CurrencyCode = string.IsNullOrWhiteSpace(model.CurrencyCode) ? "PKR" : model.CurrencyCode.Trim();
        entity.DurationNights = model.DurationNights;
        entity.SortOrder = model.SortOrder;
        entity.IsFeatured = model.IsFeatured;
        entity.IsActive = model.IsActive;
        return entity;
    }
}
