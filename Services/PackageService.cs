using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class PackageService : IPackageService
{
    private readonly ApplicationDbContext _db;
    public PackageService(ApplicationDbContext db) => _db = db;

    public async Task<(List<TravelPackage> Items, int Total)> GetPagedAsync(string? q, string? type, int page, int pageSize)
    {
        var query = _db.TravelPackages.AsQueryable();
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(p => p.PackageType == type);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.PackageName.Contains(q) || p.PackageCode.Contains(q));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.PackageId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<TravelPackage?> GetByIdAsync(int id) => _db.TravelPackages.FirstOrDefaultAsync(p => p.PackageId == id);

    public async Task<PackageEditViewModel?> GetEditAsync(int id)
    {
        var package = await _db.TravelPackages.FirstOrDefaultAsync(p => p.PackageId == id);
        if (package == null) return null;

        var destinationRows = await _db.PackageDestinations.Include(d => d.Destination)
            .Where(d => d.PackageId == id).OrderBy(d => d.SequenceNo).ToListAsync();
        var itineraryRows = await _db.PackageItineraries.Include(i => i.Destination)
            .Where(i => i.PackageId == id).OrderBy(i => i.DayNumber).ToListAsync();
        var serviceRows = await _db.PackageServices.Include(s => s.Service).Where(s => s.PackageId == id).ToListAsync();
        var departureRows = await _db.PackageDepartures.Where(d => d.PackageId == id).OrderBy(d => d.DepartureDate).ToListAsync();

        return new PackageEditViewModel
        {
            Basic = new PackageBasicInputModel
            {
                PackageId = package.PackageId,
                PackageCode = package.PackageCode,
                PackageName = package.PackageName,
                PackageType = package.PackageType,
                Description = package.Description,
                DurationDays = package.DurationDays,
                DurationNights = package.DurationNights,
                AdultPrice = package.AdultPrice,
                ChildPrice = package.ChildPrice,
                InfantPrice = package.InfantPrice,
                CurrencyCode = package.CurrencyCode,
                MinimumPersons = package.MinimumPersons,
                MaximumPersons = package.MaximumPersons,
                IsFeatured = package.IsFeatured == true,
                IsActive = package.IsActive != false
            },
            Destinations = destinationRows.Select(d => new PackageDestinationInputModel
            {
                PackageDestinationId = d.PackageDestinationId,
                DestinationId = d.DestinationId,
                DestinationName = d.Destination?.DestinationName,
                SequenceNo = d.SequenceNo ?? 1,
                NumberOfDays = d.NumberOfDays ?? 1
            }).ToList(),
            Itineraries = itineraryRows.Select(i => new PackageItineraryInputModel
            {
                ItineraryId = i.ItineraryId,
                DayNumber = i.DayNumber,
                Title = i.Title,
                Description = i.Description,
                DestinationId = i.DestinationId,
                DestinationName = i.Destination?.DestinationName
            }).ToList(),
            Services = serviceRows.Select(s => new PackageServiceInputModel
            {
                PackageServiceId = s.PackageServiceId,
                ServiceId = s.ServiceId,
                ServiceName = s.Service?.ServiceName,
                IsIncluded = s.IsIncluded != false,
                AdditionalPrice = s.AdditionalPrice ?? 0,
                Remarks = s.Remarks
            }).ToList(),
            Departures = departureRows.Select(d => new PackageDepartureInputModel
            {
                DepartureId = d.DepartureId,
                DepartureDate = d.DepartureDate,
                ReturnDate = d.ReturnDate,
                TotalSeats = d.TotalSeats ?? 0,
                AvailableSeats = d.AvailableSeats ?? 0,
                AdultPrice = d.AdultPrice,
                ChildPrice = d.ChildPrice,
                CurrencyCode = d.CurrencyCode,
                DepartureStatus = d.DepartureStatus
            }).ToList(),
            DestinationOptions = await _db.Destinations.Where(d => d.IsActive != false).OrderBy(d => d.DestinationName).ToListAsync(),
            ServiceOptions = await _db.Services.Where(s => s.IsActive != false).OrderBy(s => s.ServiceName).ToListAsync()
        };
    }

    public async Task<TravelPackage> SaveAsync(TravelPackage entity)
    {
        if (entity.PackageId == 0)
        {
            entity.CreatedDate = DateTime.Now;
            entity.IsActive ??= true;
            _db.TravelPackages.Add(entity);
        }
        else _db.TravelPackages.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task SoftDeleteAsync(int id)
    {
        var e = await _db.TravelPackages.FindAsync(id);
        if (e == null) return;
        e.IsActive = false;
        await _db.SaveChangesAsync();
    }

    public async Task SaveDepartureAsync(PackageDeparture departure)
    {
        if (departure.DepartureId == 0) _db.PackageDepartures.Add(departure);
        else _db.PackageDepartures.Update(departure);
        await _db.SaveChangesAsync();
    }

    public async Task SaveItineraryAsync(PackageItinerary itinerary)
    {
        if (itinerary.ItineraryId == 0) _db.PackageItineraries.Add(itinerary);
        else _db.PackageItineraries.Update(itinerary);
        await _db.SaveChangesAsync();
    }

    public async Task SavePackageServiceAsync(Models.Entities.PackageService packageService)
    {
        if (packageService.PackageServiceId == 0) _db.PackageServices.Add(packageService);
        else _db.PackageServices.Update(packageService);
        await _db.SaveChangesAsync();
    }

    public Task<List<PackageDeparture>> GetDeparturesAsync(int packageId) =>
        _db.PackageDepartures.Where(d => d.PackageId == packageId).OrderBy(d => d.DepartureDate).ToListAsync();
}
