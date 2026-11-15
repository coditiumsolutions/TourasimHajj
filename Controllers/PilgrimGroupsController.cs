using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[Authorize]
public class PilgrimGroupsController : Controller
{
    private readonly IHajjService _hajj;

    public PilgrimGroupsController(IHajjService hajj) => _hajj = hajj;

    public async Task<IActionResult> Index(int? hajjSeasonId, string? status, CancellationToken cancellationToken)
    {
        var groups = await _hajj.GetGroupsAsync(hajjSeasonId, status, cancellationToken);
        var seasons = await _hajj.GetSeasonsAsync(includeInactive: false, cancellationToken);
        return View(new PilgrimGroupListViewModel
        {
            Groups = groups,
            HajjSeasonId = hajjSeasonId,
            Status = status,
            SeasonOptions = seasons
        });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await _hajj.GetGroupDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = await _hajj.GetGroupFormOptionsAsync(cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PilgrimGroupInputModel model, CancellationToken cancellationToken)
    {
        model = await _hajj.GetGroupFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.CreateGroupAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not create group."); return View(model); }
        TempData["Success"] = "Pilgrim group created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var group = await _hajj.GetGroupByIdAsync(id, cancellationToken);
        if (group is null) return NotFound();
        var model = new PilgrimGroupInputModel
        {
            PilgrimGroupId = group.PilgrimGroupId,
            HajjSeasonId = group.HajjSeasonId,
            DepartureId = group.DepartureId,
            GroupCode = group.GroupCode,
            GroupName = group.GroupName,
            GroupLeaderName = group.GroupLeaderName,
            GroupLeaderMobile = group.GroupLeaderMobile,
            GroupStatus = group.GroupStatus,
            Remarks = group.Remarks
        };
        model = await _hajj.GetGroupFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PilgrimGroupInputModel model, CancellationToken cancellationToken)
    {
        model = await _hajj.GetGroupFormOptionsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.UpdateGroupAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update group."); return View(model); }
        TempData["Success"] = "Pilgrim group updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var group = await _hajj.GetGroupByIdAsync(id, cancellationToken);
        if (group is null) return NotFound();
        return View(group);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _hajj.DeleteGroupAsync(id, cancellationToken);
        TempData["Success"] = "Pilgrim group removed.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddMember(int id, CancellationToken cancellationToken)
    {
        var model = await _hajj.GetMemberFormOptionsAsync(id, cancellationToken: cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(PilgrimGroupMemberInputModel model, CancellationToken cancellationToken)
    {
        model = await _hajj.GetMemberFormOptionsAsync(model.PilgrimGroupId, model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.AddMemberAsync(model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not add member."); return View(model); }
        TempData["Success"] = "Member added.";
        return RedirectToAction(nameof(Details), new { id = model.PilgrimGroupId });
    }

    public async Task<IActionResult> EditMember(int id, CancellationToken cancellationToken)
    {
        var member = await _hajj.GetMemberByIdAsync(id, cancellationToken);
        if (member is null) return NotFound();
        var model = new PilgrimGroupMemberInputModel
        {
            PilgrimGroupMemberId = member.PilgrimGroupMemberId,
            PilgrimGroupId = member.PilgrimGroupId,
            BookingTravelerId = member.BookingTravelerId,
            IsGroupLeader = member.IsGroupLeader == true,
            RoomGroup = member.RoomGroup,
            TransportGroup = member.TransportGroup,
            Remarks = member.Remarks
        };
        model = await _hajj.GetMemberFormOptionsAsync(member.PilgrimGroupId, model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMember(int id, PilgrimGroupMemberInputModel model, CancellationToken cancellationToken)
    {
        model = await _hajj.GetMemberFormOptionsAsync(model.PilgrimGroupId, model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var (success, error) = await _hajj.UpdateMemberAsync(id, model, cancellationToken);
        if (!success) { ModelState.AddModelError(string.Empty, error ?? "Could not update member."); return View(model); }
        TempData["Success"] = "Member updated.";
        return RedirectToAction(nameof(Details), new { id = model.PilgrimGroupId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMember(int id, int pilgrimGroupId, CancellationToken cancellationToken)
    {
        await _hajj.RemoveMemberAsync(id, cancellationToken);
        TempData["Success"] = "Member removed.";
        return RedirectToAction(nameof(Details), new { id = pilgrimGroupId });
    }
}