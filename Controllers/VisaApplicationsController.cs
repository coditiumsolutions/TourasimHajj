using Microsoft.AspNetCore.Authorization;
using TourismHajj.Helpers;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize(Policy = AppRoles.AdminOnly)]
public class VisaApplicationsController : Controller
{
    private readonly IUmrahService _umrah;

    public VisaApplicationsController(IUmrahService umrah) => _umrah = umrah;

    public async Task<IActionResult> Index(string? status, CancellationToken cancellationToken)
    {
        var apps = await _umrah.GetVisaApplicationsAsync(status, cancellationToken);
        var alerts = await _umrah.GetPendingVisaAlertsAsync(14, cancellationToken);
        ViewBag.PendingAlerts = alerts;
        return View(new VisaApplicationListViewModel { Applications = apps, StatusFilter = status });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _umrah.GetVisaFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VisaApplicationInputModel model, CancellationToken cancellationToken)
    {
        model = await _umrah.GetVisaFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _umrah.CreateVisaApplicationAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Visa application created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _umrah.GetVisaApplicationByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = new VisaApplicationInputModel
        {
            VisaApplicationId = entity.VisaApplicationId,
            BookingTravelerId = entity.BookingTravelerId,
            VisaType = entity.VisaType,
            CountryId = entity.CountryId,
            ApplicationDate = entity.ApplicationDate,
            ApplicationNumber = entity.ApplicationNumber,
            VisaNumber = entity.VisaNumber,
            SubmissionDate = entity.SubmissionDate,
            ApprovalDate = entity.ApprovalDate,
            VisaIssueDate = entity.VisaIssueDate,
            VisaExpiryDate = entity.VisaExpiryDate,
            VisaStatus = entity.VisaStatus,
            SupplierId = entity.SupplierId,
            Remarks = entity.Remarks
        };
        model = await _umrah.GetVisaFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VisaApplicationInputModel model, CancellationToken cancellationToken)
    {
        model = await _umrah.GetVisaFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _umrah.UpdateVisaApplicationAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View(model); }
        TempData["Success"] = "Visa application updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(VisaStatusUpdateModel model, CancellationToken cancellationToken)
    {
        await _umrah.UpdateVisaStatusAsync(model, cancellationToken);
        TempData["Success"] = "Visa status updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _umrah.GetVisaApplicationByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _umrah.DeleteVisaApplicationAsync(id, cancellationToken);
        TempData["Success"] = "Visa application deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RoomAllocations(int? hotelId, CancellationToken cancellationToken)
    {
        var list = await _umrah.GetRoomAllocationsAsync(hotelId, cancellationToken);
        return View(new RoomAllocationListViewModel { Allocations = list, HotelId = hotelId });
    }

    public async Task<IActionResult> CreateRoom(CancellationToken cancellationToken)
    {
        var model = await _umrah.GetRoomAllocationFormOptionsAsync(cancellationToken: cancellationToken);
        return View("RoomAllocationCreate", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(RoomAllocationInputModel model, CancellationToken cancellationToken)
    {
        model = await _umrah.GetRoomAllocationFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View("RoomAllocationCreate", model);
        var (success, error) = await _umrah.CreateRoomAllocationAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not save."); return View("RoomAllocationCreate", model); }
        TempData["Success"] = "Room allocation saved.";
        return RedirectToAction(nameof(RoomAllocations));
    }
}