using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class HajjService : IHajjService
{
    private readonly ApplicationDbContext _db;

    public HajjService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<HajjSeason>> GetSeasonsAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _db.HajjSeasons.AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(s => s.IsActive != false);
        return await query.OrderByDescending(s => s.HajjYear).ThenBy(s => s.SeasonName).ToListAsync(cancellationToken);
    }

    public async Task<HajjSeason?> GetSeasonByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.HajjSeasons.FirstOrDefaultAsync(s => s.HajjSeasonId == id, cancellationToken);

    public async Task<(bool Success, string? Error)> CreateSeasonAsync(HajjSeasonInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapSeason(model, new HajjSeason());
        _db.HajjSeasons.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateSeasonAsync(int id, HajjSeasonInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.HajjSeasons.FirstOrDefaultAsync(s => s.HajjSeasonId == id, cancellationToken);
        if (entity is null) return (false, "Season not found.");
        MapSeason(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteSeasonAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.HajjSeasons.FirstOrDefaultAsync(s => s.HajjSeasonId == id, cancellationToken);
        if (entity is null) return (false, "Season not found.");
        var hasGroups = await _db.PilgrimGroups.AnyAsync(g => g.HajjSeasonId == id, cancellationToken);
        if (hasGroups)
        {
            entity.IsActive = false;
        }
        else
        {
            _db.HajjSeasons.Remove(entity);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<PilgrimGroup>> GetGroupsAsync(int? hajjSeasonId, string? status, CancellationToken cancellationToken = default)
    {
        var query = _db.PilgrimGroups.AsNoTracking()
            .Include(g => g.HajjSeason)
            .Include(g => g.Departure)
            .AsQueryable();
        if (hajjSeasonId.HasValue)
            query = query.Where(g => g.HajjSeasonId == hajjSeasonId);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(g => g.GroupStatus == status);
        return await query.OrderByDescending(g => g.PilgrimGroupId).ToListAsync(cancellationToken);
    }

    public async Task<PilgrimGroup?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.PilgrimGroups
            .Include(g => g.HajjSeason)
            .Include(g => g.Departure)
            .FirstOrDefaultAsync(g => g.PilgrimGroupId == id, cancellationToken);

    public async Task<PilgrimGroupDetailsViewModel?> GetGroupDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var group = await _db.PilgrimGroups.AsNoTracking()
            .Include(g => g.HajjSeason)
            .Include(g => g.Departure)
            .FirstOrDefaultAsync(g => g.PilgrimGroupId == id, cancellationToken);
        if (group is null) return null;

        var members = await _db.PilgrimGroupMembers.AsNoTracking()
            .Include(m => m.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(m => m.BookingTraveler).ThenInclude(bt => bt.Booking)
            .Where(m => m.PilgrimGroupId == id)
            .OrderByDescending(m => m.IsGroupLeader == true)
            .ThenBy(m => m.PilgrimGroupMemberId)
            .ToListAsync(cancellationToken);

        return new PilgrimGroupDetailsViewModel { Group = group, Members = members };
    }

    public async Task<(bool Success, string? Error)> CreateGroupAsync(PilgrimGroupInputModel model, CancellationToken cancellationToken = default)
    {
        if (await _db.PilgrimGroups.AnyAsync(g => g.GroupCode == model.GroupCode, cancellationToken))
            return (false, "Group code already exists.");

        var entity = MapGroup(model, new PilgrimGroup());
        _db.PilgrimGroups.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await RefreshGroupTotalAsync(entity.PilgrimGroupId, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateGroupAsync(int id, PilgrimGroupInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PilgrimGroups.FirstOrDefaultAsync(g => g.PilgrimGroupId == id, cancellationToken);
        if (entity is null) return (false, "Group not found.");
        if (await _db.PilgrimGroups.AnyAsync(g => g.GroupCode == model.GroupCode && g.PilgrimGroupId != id, cancellationToken))
            return (false, "Group code already exists.");

        MapGroup(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteGroupAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PilgrimGroups.Include(g => g.PilgrimGroupMembers).FirstOrDefaultAsync(g => g.PilgrimGroupId == id, cancellationToken);
        if (entity is null) return (false, "Group not found.");
        if (entity.PilgrimGroupMembers.Count > 0)
            return (false, "Remove group members before deleting the group.");

        _db.PilgrimGroups.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<PilgrimGroupMember?> GetMemberByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.PilgrimGroupMembers
            .Include(m => m.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .FirstOrDefaultAsync(m => m.PilgrimGroupMemberId == id, cancellationToken);

    public async Task<(bool Success, string? Error)> AddMemberAsync(PilgrimGroupMemberInputModel model, CancellationToken cancellationToken = default)
    {
        if (await _db.PilgrimGroupMembers.AnyAsync(m => m.PilgrimGroupId == model.PilgrimGroupId && m.BookingTravelerId == model.BookingTravelerId, cancellationToken))
            return (false, "Traveler is already in this group.");

        var entity = MapMember(model, new PilgrimGroupMember());
        _db.PilgrimGroupMembers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await RefreshGroupTotalAsync(model.PilgrimGroupId, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateMemberAsync(int id, PilgrimGroupMemberInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PilgrimGroupMembers.FirstOrDefaultAsync(m => m.PilgrimGroupMemberId == id, cancellationToken);
        if (entity is null) return (false, "Member not found.");
        if (await _db.PilgrimGroupMembers.AnyAsync(m => m.PilgrimGroupId == model.PilgrimGroupId && m.BookingTravelerId == model.BookingTravelerId && m.PilgrimGroupMemberId != id, cancellationToken))
            return (false, "Traveler is already in this group.");

        MapMember(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RemoveMemberAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.PilgrimGroupMembers.FirstOrDefaultAsync(m => m.PilgrimGroupMemberId == id, cancellationToken);
        if (entity is null) return (false, "Member not found.");
        var groupId = entity.PilgrimGroupId;
        _db.PilgrimGroupMembers.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await RefreshGroupTotalAsync(groupId, cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<HajjPilgrimRegisterRow>> GetHajjPilgrimRegisterAsync(int? seasonId, int? groupId, CancellationToken cancellationToken = default)
    {
        var query = _db.PilgrimGroupMembers.AsNoTracking()
            .Include(m => m.PilgrimGroup).ThenInclude(g => g.HajjSeason)
            .Include(m => m.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(m => m.BookingTraveler).ThenInclude(bt => bt.Booking)
            .AsQueryable();

        if (seasonId.HasValue)
            query = query.Where(m => m.PilgrimGroup.HajjSeasonId == seasonId);
        if (groupId.HasValue)
            query = query.Where(m => m.PilgrimGroupId == groupId);

        return await query.OrderBy(m => m.PilgrimGroup.GroupCode).ThenBy(m => m.BookingTraveler.Traveler.FullName)
            .Select(m => new HajjPilgrimRegisterRow
            {
                SeasonName = m.PilgrimGroup.HajjSeason.SeasonName,
                GroupCode = m.PilgrimGroup.GroupCode,
                GroupName = m.PilgrimGroup.GroupName,
                PilgrimName = m.BookingTraveler.Traveler.FullName,
                PassportNo = m.BookingTraveler.Traveler.PassportNo,
                MobileNo = m.BookingTraveler.Traveler.MobileNo,
                BookingNo = m.BookingTraveler.Booking.BookingNo,
                IsGroupLeader = m.IsGroupLeader == true,
                RoomGroup = m.RoomGroup,
                TransportGroup = m.TransportGroup
            }).ToListAsync(cancellationToken);
    }

    public async Task<PilgrimGroupInputModel> GetGroupFormOptionsAsync(PilgrimGroupInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new PilgrimGroupInputModel();
        model.SeasonOptions = await _db.HajjSeasons.AsNoTracking().Where(s => s.IsActive != false).OrderByDescending(s => s.HajjYear).ToListAsync(cancellationToken);
        model.DepartureOptions = await _db.PackageDepartures.AsNoTracking()
            .Include(d => d.Package)
            .Where(d => d.Package.PackageType == "Hajj")
            .OrderByDescending(d => d.DepartureDate)
            .Take(100)
            .ToListAsync(cancellationToken);
        return model;
    }

    public async Task<PilgrimGroupMemberInputModel> GetMemberFormOptionsAsync(int pilgrimGroupId, PilgrimGroupMemberInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new PilgrimGroupMemberInputModel { PilgrimGroupId = pilgrimGroupId };
        model.PilgrimGroupId = pilgrimGroupId;
        model.GroupCode = await _db.PilgrimGroups.Where(g => g.PilgrimGroupId == pilgrimGroupId).Select(g => g.GroupCode).FirstOrDefaultAsync(cancellationToken);
        model.TravelerOptions = await GetHajjTravelerOptionsAsync(cancellationToken);
        return model;
    }

    private async Task<IReadOnlyList<BookingTravelerOption>> GetHajjTravelerOptionsAsync(CancellationToken cancellationToken)
    {
        return await _db.BookingTravelers.AsNoTracking()
            .Include(bt => bt.Traveler)
            .Include(bt => bt.Booking).ThenInclude(b => b.Package)
            .Where(bt => bt.Booking.Package.PackageType == "Hajj" && bt.Booking.BookingStatus != "Cancelled")
            .OrderByDescending(bt => bt.BookingTravelerId)
            .Take(500)
            .Select(bt => new BookingTravelerOption
            {
                BookingTravelerId = bt.BookingTravelerId,
                DisplayName = $"{bt.Traveler.FullName} ({bt.Booking.BookingNo})"
            }).ToListAsync(cancellationToken);
    }

    private async Task RefreshGroupTotalAsync(int pilgrimGroupId, CancellationToken cancellationToken)
    {
        var group = await _db.PilgrimGroups.FirstOrDefaultAsync(g => g.PilgrimGroupId == pilgrimGroupId, cancellationToken);
        if (group is null) return;
        group.TotalPilgrims = await _db.PilgrimGroupMembers.CountAsync(m => m.PilgrimGroupId == pilgrimGroupId, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static HajjSeason MapSeason(HajjSeasonInputModel model, HajjSeason entity)
    {
        entity.SeasonName = model.SeasonName.Trim();
        entity.HajjYear = model.HajjYear;
        entity.RegistrationStartDate = model.RegistrationStartDate;
        entity.RegistrationEndDate = model.RegistrationEndDate;
        entity.DepartureStartDate = model.DepartureStartDate;
        entity.DepartureEndDate = model.DepartureEndDate;
        entity.Notes = model.Notes?.Trim();
        entity.IsActive = model.IsActive;
        return entity;
    }

    private static PilgrimGroup MapGroup(PilgrimGroupInputModel model, PilgrimGroup entity)
    {
        entity.HajjSeasonId = model.HajjSeasonId;
        entity.DepartureId = model.DepartureId;
        entity.GroupCode = model.GroupCode.Trim();
        entity.GroupName = model.GroupName?.Trim();
        entity.GroupLeaderName = model.GroupLeaderName?.Trim();
        entity.GroupLeaderMobile = model.GroupLeaderMobile?.Trim();
        entity.GroupStatus = string.IsNullOrWhiteSpace(model.GroupStatus) ? "Planning" : model.GroupStatus.Trim();
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }

    private static PilgrimGroupMember MapMember(PilgrimGroupMemberInputModel model, PilgrimGroupMember entity)
    {
        entity.PilgrimGroupId = model.PilgrimGroupId;
        entity.BookingTravelerId = model.BookingTravelerId;
        entity.IsGroupLeader = model.IsGroupLeader;
        entity.RoomGroup = model.RoomGroup?.Trim();
        entity.TransportGroup = model.TransportGroup?.Trim();
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }
}