using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    private readonly IMasterDataService _masters;

    public CustomersController(ICustomerService service, IMasterDataService masters)
    {
        _service = service;
        _masters = masters;
    }

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        ViewData["Title"] = "Customers";
        var (items, total) = await _service.GetPagedAsync(q, page, 10);
        ViewBag.Q = q;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.PageSize = 10;
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
        await LoadLookups();
        return View(new CustomerInputModel
        {
            IsActive = true,
            CustomerType = "Individual",
            Members = { new CustomerGroupMemberInputModel() }
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "New Customer";
            await LoadLookups();
            EnsureMembers(model);
            return View(model);
        }

        var (success, error, customerId) = await _service.SaveWithGroupAsync(model, cancellationToken);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Could not save customer.");
            ViewData["Title"] = "New Customer";
            await LoadLookups();
            EnsureMembers(model);
            return View(model);
        }

        TempData["Success"] = "Customer and group saved.";
        return RedirectToAction(nameof(Details), new { id = customerId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Customer";
        var model = await _service.GetFormAsync(id, cancellationToken);
        if (model == null) return NotFound();
        await LoadLookups();
        EnsureMembers(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerInputModel model, CancellationToken cancellationToken)
    {
        if (id != model.CustomerId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Customer";
            await LoadLookups();
            EnsureMembers(model);
            return View(model);
        }

        var (success, error, customerId) = await _service.SaveWithGroupAsync(model, cancellationToken);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Could not save customer.");
            ViewData["Title"] = "Edit Customer";
            await LoadLookups();
            EnsureMembers(model);
            return View(model);
        }

        TempData["Success"] = "Customer and group updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteAsync(id);
        TempData["Success"] = "Customer deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups()
    {
        ViewBag.Cities = new SelectList(await _masters.GetActiveCitiesListAsync(), nameof(City.CityId), nameof(City.CityName));
    }

    private static void EnsureMembers(CustomerInputModel model)
    {
        model.Members ??= new List<CustomerGroupMemberInputModel>();
        if (model.Members.Count == 0)
            model.Members.Add(new CustomerGroupMemberInputModel());
    }
}
