using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _db;

    public SupplierService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Supplier>> GetSuppliersAsync(string? typeFilter, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _db.Suppliers.AsNoTracking().Include(s => s.City).Include(s => s.Country).AsQueryable();
        if (!includeInactive)
            query = query.Where(s => s.IsActive != false);
        if (!string.IsNullOrWhiteSpace(typeFilter))
            query = query.Where(s => s.SupplierType == typeFilter);
        return await query.OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Suppliers.Include(s => s.City).Include(s => s.Country).FirstOrDefaultAsync(s => s.SupplierId == id, cancellationToken);

    public async Task<SupplierInputModel> GetSupplierFormOptionsAsync(SupplierInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new SupplierInputModel();
        model.CountryOptions = await _db.Countries.AsNoTracking().Where(c => c.IsActive != false).OrderBy(c => c.CountryName).ToListAsync(cancellationToken);
        model.CityOptions = await _db.Cities.AsNoTracking().Where(c => c.IsActive != false).OrderBy(c => c.CityName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateSupplierAsync(SupplierInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapSupplier(model, new Supplier());
        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateSupplierAsync(int id, SupplierInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id, cancellationToken);
        if (entity is null) return (false, "Supplier not found.");
        MapSupplier(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteSupplierAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id, cancellationToken);
        if (entity is null) return (false, "Supplier not found.");
        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<Hotel>> GetHotelsAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _db.Hotels.AsNoTracking().Include(h => h.City).Include(h => h.Supplier).AsQueryable();
        if (!includeInactive)
            query = query.Where(h => h.IsActive != false);
        return await query.OrderBy(h => h.HotelName).ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Hotels.Include(h => h.City).Include(h => h.Supplier).FirstOrDefaultAsync(h => h.HotelId == id, cancellationToken);

    public async Task<HotelInputModel> GetHotelFormOptionsAsync(HotelInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new HotelInputModel();
        model.SupplierOptions = await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
        model.CityOptions = await _db.Cities.AsNoTracking().Where(c => c.IsActive != false).OrderBy(c => c.CityName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateHotelAsync(HotelInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapHotel(model, new Hotel());
        _db.Hotels.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateHotelAsync(int id, HotelInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Hotels.FirstOrDefaultAsync(h => h.HotelId == id, cancellationToken);
        if (entity is null) return (false, "Hotel not found.");
        MapHotel(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteHotelAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Hotels.FirstOrDefaultAsync(h => h.HotelId == id, cancellationToken);
        if (entity is null) return (false, "Hotel not found.");
        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<TransportProvider>> GetTransportProvidersAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _db.TransportProviders.AsNoTracking().Include(t => t.Supplier).AsQueryable();
        if (!includeInactive)
            query = query.Where(t => t.IsActive != false);
        return await query.OrderBy(t => t.ProviderName).ToListAsync(cancellationToken);
    }

    public async Task<TransportProvider?> GetTransportProviderByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.TransportProviders.Include(t => t.Supplier).FirstOrDefaultAsync(t => t.TransportProviderId == id, cancellationToken);

    public async Task<TransportProviderInputModel> GetTransportProviderFormOptionsAsync(TransportProviderInputModel? model = null, CancellationToken cancellationToken = default)
    {
        model ??= new TransportProviderInputModel();
        model.SupplierOptions = await _db.Suppliers.AsNoTracking().Where(s => s.IsActive != false).OrderBy(s => s.SupplierName).ToListAsync(cancellationToken);
        return model;
    }

    public async Task<(bool Success, string? Error)> CreateTransportProviderAsync(TransportProviderInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = MapTransport(model, new TransportProvider());
        _db.TransportProviders.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateTransportProviderAsync(int id, TransportProviderInputModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TransportProviders.FirstOrDefaultAsync(t => t.TransportProviderId == id, cancellationToken);
        if (entity is null) return (false, "Transport provider not found.");
        MapTransport(model, entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteTransportProviderAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TransportProviders.FirstOrDefaultAsync(t => t.TransportProviderId == id, cancellationToken);
        if (entity is null) return (false, "Transport provider not found.");
        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private static Supplier MapSupplier(SupplierInputModel model, Supplier entity)
    {
        entity.SupplierName = model.SupplierName.Trim();
        entity.SupplierType = model.SupplierType.Trim();
        entity.ContactPerson = model.ContactPerson?.Trim();
        entity.MobileNo = model.MobileNo?.Trim();
        entity.Email = model.Email?.Trim();
        entity.CountryId = model.CountryId;
        entity.CityId = model.CityId;
        entity.Address = model.Address?.Trim();
        entity.IsActive = model.IsActive;
        return entity;
    }

    private static Hotel MapHotel(HotelInputModel model, Hotel entity)
    {
        entity.SupplierId = model.SupplierId;
        entity.CityId = model.CityId;
        entity.HotelName = model.HotelName.Trim();
        entity.StarRating = model.StarRating;
        entity.Address = model.Address?.Trim();
        entity.DistanceFromHaramKm = model.DistanceFromHaramKm;
        entity.DistanceFromNabawiKm = model.DistanceFromNabawiKm;
        entity.ContactNo = model.ContactNo?.Trim();
        entity.IsActive = model.IsActive;
        return entity;
    }

    private static TransportProvider MapTransport(TransportProviderInputModel model, TransportProvider entity)
    {
        entity.SupplierId = model.SupplierId;
        entity.ProviderName = model.ProviderName.Trim();
        entity.VehicleType = model.VehicleType?.Trim();
        entity.VehicleCapacity = model.VehicleCapacity;
        entity.ContactNo = model.ContactNo?.Trim();
        entity.IsActive = model.IsActive;
        return entity;
    }
}