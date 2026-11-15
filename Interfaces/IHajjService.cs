using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IHajjService
{
    Task<IReadOnlyList<HajjSeason>> GetSeasonsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<HajjSeason?> GetSeasonByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateSeasonAsync(HajjSeasonInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateSeasonAsync(int id, HajjSeasonInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteSeasonAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PilgrimGroup>> GetGroupsAsync(int? hajjSeasonId, string? status, CancellationToken cancellationToken = default);
    Task<PilgrimGroup?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PilgrimGroupDetailsViewModel?> GetGroupDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateGroupAsync(PilgrimGroupInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateGroupAsync(int id, PilgrimGroupInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteGroupAsync(int id, CancellationToken cancellationToken = default);

    Task<PilgrimGroupMember?> GetMemberByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> AddMemberAsync(PilgrimGroupMemberInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateMemberAsync(int id, PilgrimGroupMemberInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> RemoveMemberAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HajjPilgrimRegisterRow>> GetHajjPilgrimRegisterAsync(int? seasonId, int? groupId, CancellationToken cancellationToken = default);
    Task<PilgrimGroupInputModel> GetGroupFormOptionsAsync(PilgrimGroupInputModel? model = null, CancellationToken cancellationToken = default);
    Task<PilgrimGroupMemberInputModel> GetMemberFormOptionsAsync(int pilgrimGroupId, PilgrimGroupMemberInputModel? model = null, CancellationToken cancellationToken = default);
}