using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class GalleryController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public GalleryController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Gallery";
        var items = await _db.GalleryItems
            .OrderBy(g => g.SortOrder)
            .ThenByDescending(g => g.GalleryItemId)
            .ToListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Add Gallery Image";
        return View(new GalleryItemInputModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GalleryItemInputModel model, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        try
        {
            var uploaded = await PackageImageStorage.SaveGalleryAsync(_env, imageFile, cancellationToken);
            if (!string.IsNullOrWhiteSpace(uploaded))
                model.ImageUrl = uploaded;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageUrl), ex.Message);
        }

        if (string.IsNullOrWhiteSpace(model.ImageUrl))
            ModelState.AddModelError(nameof(model.ImageUrl), "Please upload an image or provide an image URL.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Add Gallery Image";
            return View(model);
        }

        _db.GalleryItems.Add(new GalleryItem
        {
            Title = model.Title.Trim(),
            Description = model.Description,
            ImageUrl = model.ImageUrl!,
            SortOrder = model.SortOrder,
            IsActive = model.IsActive,
            CreatedDate = DateTime.Now
        });
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Gallery image added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var item = await _db.GalleryItems.FindAsync([id], cancellationToken);
        if (item == null) return NotFound();

        ViewData["Title"] = "Edit Gallery Image";
        return View(new GalleryItemInputModel
        {
            GalleryItemId = item.GalleryItemId,
            Title = item.Title,
            Description = item.Description,
            ImageUrl = item.ImageUrl,
            SortOrder = item.SortOrder,
            IsActive = item.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GalleryItemInputModel model, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        var item = await _db.GalleryItems.FindAsync([id], cancellationToken);
        if (item == null) return NotFound();

        try
        {
            var uploaded = await PackageImageStorage.SaveGalleryAsync(_env, imageFile, cancellationToken);
            if (!string.IsNullOrWhiteSpace(uploaded))
                model.ImageUrl = uploaded;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageUrl), ex.Message);
        }

        if (string.IsNullOrWhiteSpace(model.ImageUrl))
            ModelState.AddModelError(nameof(model.ImageUrl), "Image URL is required.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Gallery Image";
            return View(model);
        }

        item.Title = model.Title.Trim();
        item.Description = model.Description;
        item.ImageUrl = model.ImageUrl!;
        item.SortOrder = model.SortOrder;
        item.IsActive = model.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Gallery image updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await _db.GalleryItems.FindAsync([id], cancellationToken);
        if (item == null) return NotFound();
        item.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Gallery image deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
