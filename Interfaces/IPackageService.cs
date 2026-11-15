using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IPackageService
{
    Task<(List<TravelPackage> Items, int Total)> GetPagedAsync(string? q, string? type, int page, int pageSize);
    Task<TravelPackage?> GetByIdAsync(int id);
    Task<PackageEditViewModel?> GetEditAsync(int id);
    Task<TravelPackage> SaveAsync(TravelPackage entity);
    Task SoftDeleteAsync(int id);
    Task SaveDepartureAsync(PackageDeparture departure);
    Task SaveItineraryAsync(PackageItinerary itinerary);
    Task SavePackageServiceAsync(Models.Entities.PackageService packageService);
    Task<List<PackageDeparture>> GetDeparturesAsync(int packageId);
}
