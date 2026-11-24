using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class SetupController : Controller
{
    private readonly ApplicationDbContext _db;

    public SetupController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Setup";
        var query = _db.AppConfigs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c =>
                c.ConfigKey.Contains(q) ||
                c.ConfigValue.Contains(q) ||
                (c.Description != null && c.Description.Contains(q)));
        }

        var items = await query
            .OrderBy(c => c.ConfigKey)
            .ThenBy(c => c.AppConfigId)
            .ToListAsync(cancellationToken);

        ViewBag.Q = q;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Add Config";
        return View(new AppConfigInputModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppConfigInputModel model, CancellationToken cancellationToken)
    {
        model.ConfigKey = model.ConfigKey?.Trim() ?? string.Empty;
        if (await _db.AppConfigs.AnyAsync(c => c.ConfigKey == model.ConfigKey, cancellationToken))
            ModelState.AddModelError(nameof(model.ConfigKey), "This Config Key already exists.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Add Config";
            return View(model);
        }

        _db.AppConfigs.Add(new AppConfig
        {
            ConfigKey = model.ConfigKey,
            ConfigValue = model.ConfigValue.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            IsActive = model.IsActive,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        });
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Config saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var item = await _db.AppConfigs.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();

        ViewData["Title"] = "Edit Config";
        return View(new AppConfigInputModel
        {
            AppConfigId = item.AppConfigId,
            ConfigKey = item.ConfigKey,
            ConfigValue = item.ConfigValue,
            Description = item.Description,
            IsActive = item.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppConfigInputModel model, CancellationToken cancellationToken)
    {
        var item = await _db.AppConfigs.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();

        model.ConfigKey = model.ConfigKey?.Trim() ?? string.Empty;
        if (await _db.AppConfigs.AnyAsync(c => c.ConfigKey == model.ConfigKey && c.AppConfigId != id, cancellationToken))
            ModelState.AddModelError(nameof(model.ConfigKey), "This Config Key already exists.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Config";
            return View(model);
        }

        item.ConfigKey = model.ConfigKey;
        item.ConfigValue = model.ConfigValue.Trim();
        item.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        item.IsActive = model.IsActive;
        item.UpdatedDate = DateTime.Now;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Config updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await _db.AppConfigs.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();
        _db.AppConfigs.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Config deleted.";
        return RedirectToAction(nameof(Index));
    }
}
