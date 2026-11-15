using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface ITravelerService
{
    Task<(List<Traveler> Items, int Total)> GetPagedAsync(string? q, int? customerId, int page, int pageSize);
    Task<Traveler?> GetByIdAsync(int id);
    Task<Traveler> SaveAsync(Traveler entity);
    Task DeleteAsync(int id);
    Task<List<PassportAlertItem>> GetPassportAlertsAsync(int days = 90);
    Task<TravelerDocument> AddDocumentAsync(TravelerDocument doc);
    Task<List<TravelerDocument>> GetDocumentsAsync(int travelerId);
}
