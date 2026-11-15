using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    private readonly IMasterDataService _masters;
    public CustomersController(ICustomerService service, IMasterDataService masters) { _service = service; _masters = masters; }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Customers";
        var (items, total) = await _service.GetPagedAsync(q, page, 10);
        ViewBag.Q = q; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = 10;
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Customer Details";
        var model = await _service.GetDetailsAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "New Customer";
        await LoadCities();
        return View(new Customer { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (!ModelState.IsValid) { await LoadCities(); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Customer created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Customer";
        var model = await _service.GetByIdAsync(id);
        if (model == null) return NotFound();
        await LoadCities();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.CustomerId) return BadRequest();
        if (!ModelState.IsValid) { await LoadCities(); return View(model); }
        await _service.SaveAsync(model);
        TempData["Success"] = "Customer updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteAsync(id);
        TempData["Success"] = "Customer deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCities()
    {
        ViewBag.Cities = new SelectList(await _masters.GetActiveCitiesListAsync(), nameof(City.CityId), nameof(City.CityName));
    }
}
