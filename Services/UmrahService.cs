using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class UmrahService : IUmrahService
{
    private readonly ApplicationDbContext _db;

    public UmrahService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<TravelPackage>> GetUmrahPackagesAsync(string? search, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _db.TravelPackages.AsNoTracking().Where(p => p.PackageType == "Umrah");
        if (!includeInactive)
            query = query.Where(p => p.IsActive != false);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.PackageName.Contains(term) || p.PackageCode.Contains(term));
        }

        return await query.OrderByDescending(p => p.PackageId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VisaApplication>> GetVisaApplicationsAsync(string? status, CancellationToken cancellationToken = default)
    {
        var query = _db.VisaApplications.AsNoTracking()
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Booking)
            .Include(v => v.Country)
            .Include(v => v.Supplier)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(v => v.VisaStatus == status);
        return await query.OrderByDescending(v => v.VisaApplicationId).ToListAsync(cancellationToken);
    }

    public async Task<VisaApplication?> GetVisaApplicationByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.VisaApplications
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Booking)
            .Include(v => v.Country)
            .Include(v => v.Supplier)
            .FirstOrDefaultAsync(v => v.VisaApplicationId == id, cancellationToken);

    public async Task<VisaApplicationInputModel> GetVisaFormOptionsAsync(VisaApplicationInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new VisaApplicationInputModel();
        model.TravelerOptions = await GetUmrahTravelerOptionsAsync(cancellationToken);
        model.CountryOptions = await _db.Countries.AsNoTracking().Where(c => c.IsActive != false).OrderBy(c => c.CountryName).ToListAsync(cancellationToken);
        model.SupplierOptions = await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateVisaApplicationAsync(VisaApplicationInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapVisa(model, new VisaApplication());
        if (entity.ApplicationDate is null)
            entity.ApplicationDate = DateOnly.FromDateTime(DateTime.Today);
        _db.VisaApplications.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateVisaApplicationAsync(int id, VisaApplicationInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(v => v.VisaApplicationId == id, cancellationToken);
        if (entity is null) return (false, "Visa application not found.");
        MapVisa(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateVisaStatusAsync(VisaStatusUpdateModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(v => v.VisaApplicationId == model.VisaApplicationId, cancellationToken);
        if (entity is null) return (false, "Visa application not found.");
        entity.VisaStatus = model.VisaStatus.Trim();
        entity.ApprovalDate = model.ApprovalDate;
        entity.VisaIssueDate = model.VisaIssueDate;
        entity.VisaExpiryDate = model.VisaExpiryDate;
        entity.VisaNumber = model.VisaNumber?.Trim();
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteVisaApplicationAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(v => v.VisaApplicationId == id, cancellationToken);
        if (entity is null) return (false, "Visa application not found.");
        _db.VisaApplications.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<RoomAllocation>> GetRoomAllocationsAsync(int? hotelId, CancellationToken cancellationToken = default)
    {
        var query = _db.RoomAllocations.AsNoTracking()
            .Include(r => r.Hotel).ThenInclude(h => h.City)
            .Include(r => r.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(r => r.BookingTraveler).ThenInclude(bt => bt.Booking)
            .AsQueryable();
        if (hotelId.HasValue)
            query = query.Where(r => r.HotelId == hotelId);
        return await query.OrderByDescending(r => r.RoomAllocationId).ToListAsync(cancellationToken);
    }

    public async Task<RoomAllocation?> GetRoomAllocationByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.RoomAllocations
            .Include(r => r.Hotel)
            .Include(r => r.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .FirstOrDefaultAsync(r => r.RoomAllocationId == id, cancellationToken);

    public async Task<RoomAllocationInputModel> GetRoomAllocationFormOptionsAsync(RoomAllocationInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new RoomAllocationInputModel();
        model.TravelerOptions = await GetUmrahTravelerOptionsAsync(cancellationToken);
        model.HotelOptions = await _db.Hotels.AsNoTracking().Include(h => h.City).Where(h => h.IsActive != false).OrderBy(h => h.HotelName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateRoomAllocationAsync(RoomAllocationInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapRoom(model, new RoomAllocation());
        _db.RoomAllocations.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateRoomAllocationAsync(int id, RoomAllocationInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.RoomAllocations.FirstOrDefaultAsync(r => r.RoomAllocationId == id, cancellationToken);
        if (entity is null) return (false, "Room allocation not found.");
        MapRoom(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteRoomAllocationAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.RoomAllocations.FirstOrDefaultAsync(r => r.RoomAllocationId == id, cancellationToken);
        if (entity is null) return (false, "Room allocation not found.");
        _db.RoomAllocations.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<PendingVisaAlertRow>> GetPendingVisaAlertsAsync(int daysThreshold = 14, CancellationToken cancellationToken = default)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-daysThreshold));
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _db.VisaApplications.AsNoTracking()
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Traveler)
            .Include(v => v.BookingTraveler).ThenInclude(bt => bt.Booking)
            .Where(v => v.VisaStatus == "Pending" && v.ApplicationDate != null && v.ApplicationDate <= cutoff)
            .OrderBy(v => v.ApplicationDate)
            .Select(v => new PendingVisaAlertRow
            {
                VisaApplicationId = v.VisaApplicationId,
                TravelerName = v.BookingTraveler.Traveler.FullName,
                BookingNo = v.BookingTraveler.Booking.BookingNo,
                VisaType = v.VisaType,
                VisaStatus = v.VisaStatus,
                ApplicationDate = v.ApplicationDate,
                DaysPending = v.ApplicationDate.HasValue ? today.DayNumber - v.ApplicationDate.Value.DayNumber : 0
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UmrahPilgrimRegisterRow>> GetUmrahPilgrimRegisterAsync(CancellationToken cancellationToken = default)
    {
        return await _db.BookingTravelers.AsNoTracking()
            .Include(bt => bt.Traveler)
            .Include(bt => bt.Booking).ThenInclude(b => b.Package)
            .Include(bt => bt.VisaApplications)
            .Include(bt => bt.RoomAllocations).ThenInclude(ra => ra.Hotel)
            .Where(bt => bt.Booking.Package.PackageType == "Umrah" && bt.Booking.BookingStatus != "Cancelled")
            .OrderBy(bt => bt.Booking.BookingNo)
            .Select(bt => new UmrahPilgrimRegisterRow
            {
                BookingNo = bt.Booking.BookingNo,
                PackageName = bt.Booking.Package.PackageName,
                PilgrimName = bt.Traveler.FullName,
                PassportNo = bt.Traveler.PassportNo,
                VisaStatus = bt.VisaApplications.OrderByDescending(v => v.VisaApplicationId).Select(v => v.VisaStatus).FirstOrDefault(),
                HotelName = bt.RoomAllocations.OrderByDescending(r => r.RoomAllocationId).Select(r => r.Hotel.HotelName).FirstOrDefault(),
                RoomNumber = bt.RoomAllocations.OrderByDescending(r => r.RoomAllocationId).Select(r => r.RoomNumber).FirstOrDefault()
            }).ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<BookingTravelerOption>> GetUmrahTravelerOptionsAsync(CancellationToken cancellationToken) =>
        await _db.BookingTravelers.AsNoTracking()
            .Include(bt => bt.Traveler)
            .Include(bt => bt.Booking).ThenInclude(b => b.Package)
            .Where(bt => bt.Booking.Package.PackageType == "Umrah" && bt.Booking.BookingStatus != "Cancelled")
            .OrderByDescending(bt => bt.BookingTravelerId)
            .Take(500)
            .Select(bt => new BookingTravelerOption
            {
                BookingTravelerId = bt.BookingTravelerId,
                DisplayName = $"{bt.Traveler.FullName} ({bt.Booking.BookingNo})"
            }).ToListAsync(cancellationToken);

    private static VisaApplication MapVisa(VisaApplicationInputModel model, VisaApplication entity)
    {
        entity.BookingTravelerId = model.BookingTravelerId;
        entity.VisaType = model.VisaType.Trim();
        entity.CountryId = model.CountryId;
        entity.ApplicationDate = model.ApplicationDate;
        entity.ApplicationNumber = model.ApplicationNumber?.Trim();
        entity.VisaNumber = model.VisaNumber?.Trim();
        entity.SubmissionDate = model.SubmissionDate;
        entity.ApprovalDate = model.ApprovalDate;
        entity.VisaIssueDate = model.VisaIssueDate;
        entity.VisaExpiryDate = model.VisaExpiryDate;
        entity.VisaStatus = string.IsNullOrWhiteSpace(model.VisaStatus) ? "Pending" : model.VisaStatus.Trim();
        entity.SupplierId = model.SupplierId;
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }

    private static RoomAllocation MapRoom(RoomAllocationInputModel model, RoomAllocation entity)
    {
        entity.BookingTravelerId = model.BookingTravelerId;
        entity.HotelId = model.HotelId;
        entity.RoomNumber = model.RoomNumber?.Trim();
        entity.RoomType = model.RoomType?.Trim();
        entity.CheckInDate = model.CheckInDate;
        entity.CheckOutDate = model.CheckOutDate;
        entity.SharingType = model.SharingType?.Trim();
        entity.Remarks = model.Remarks?.Trim();
        return entity;
    }
}