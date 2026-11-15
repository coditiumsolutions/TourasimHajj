using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IUmrahService
{
    Task<IReadOnlyList<TravelPackage>> GetUmrahPackagesAsync(string? search, bool includeInactive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisaApplication>> GetVisaApplicationsAsync(string? status, CancellationToken cancellationToken = default);
    Task<VisaApplication?> GetVisaApplicationByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VisaApplicationInputModel> GetVisaFormOptionsAsync(VisaApplicationInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateVisaApplicationAsync(VisaApplicationInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateVisaApplicationAsync(int id, VisaApplicationInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateVisaStatusAsync(VisaStatusUpdateModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteVisaApplicationAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomAllocation>> GetRoomAllocationsAsync(int? hotelId, CancellationToken cancellationToken = default);
    Task<RoomAllocation?> GetRoomAllocationByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoomAllocationInputModel> GetRoomAllocationFormOptionsAsync(RoomAllocationInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateRoomAllocationAsync(RoomAllocationInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateRoomAllocationAsync(int id, RoomAllocationInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteRoomAllocationAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PendingVisaAlertRow>> GetPendingVisaAlertsAsync(int daysThreshold = 14, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UmrahPilgrimRegisterRow>> GetUmrahPilgrimRegisterAsync(CancellationToken cancellationToken = default);
}