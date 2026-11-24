using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class TravelerService : ITravelerService
{
    private readonly ApplicationDbContext _db;
    public TravelerService(ApplicationDbContext db) => _db = db;

    public async Task<(List<Traveler> Items, int Total)> GetPagedAsync(string? q, int? customerId, int page, int pageSize)
    {
        var query = _db.Travelers.Include(t => t.Customer).Include(t => t.NationalityCountry).AsQueryable();
        if (customerId.HasValue) query = query.Where(t => t.CustomerId == customerId);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(t => t.FullName.Contains(q) || (t.PassportNo != null && t.PassportNo.Contains(q)) || (t.Cnic != null && t.Cnic.Contains(q)));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(t => t.TravelerId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Traveler?> GetByIdAsync(int id) =>
        _db.Travelers.Include(t => t.Customer).Include(t => t.NationalityCountry)
            .Include(t => t.TravelerDocuments)
            .FirstOrDefaultAsync(t => t.TravelerId == id);

    public async Task<Traveler> SaveAsync(Traveler entity)
    {
        if (entity.TravelerId == 0)
        {
            entity.CreatedDate = DateTime.Now;
            _db.Travelers.Add(entity);
        }
        else _db.Travelers.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _db.Travelers.FindAsync(id);
        if (e == null) return;
        _db.Travelers.Remove(e);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PassportAlertItem>> GetPassportAlertsAsync(int days = 90)
    {
        var limit = DateOnly.FromDateTime(DateTime.Today.AddDays(days));
        return await _db.Travelers
            .Where(t => t.PassportExpiryDate != null && t.PassportExpiryDate <= limit)
            .OrderBy(t => t.PassportExpiryDate)
            .Select(t => new PassportAlertItem
            {
                TravelerId = t.TravelerId,
                FullName = t.FullName,
                PassportNo = t.PassportNo,
                ExpiryDate = t.PassportExpiryDate
            }).ToListAsync();
    }

    public async Task<TravelerDocument> AddDocumentAsync(TravelerDocument doc)
    {
        doc.UploadedDate = DateTime.Now;
        _db.TravelerDocuments.Add(doc);
        await _db.SaveChangesAsync();
        return doc;
    }

    public Task<List<TravelerDocument>> GetDocumentsAsync(int travelerId) =>
        _db.TravelerDocuments.Where(d => d.TravelerId == travelerId).OrderByDescending(d => d.UploadedDate).ToListAsync();

    public async Task<TravelerDocumentsPageViewModel> GetDocumentsPageAsync(
        int? travelerId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var travelerQuery = _db.Travelers.AsNoTracking()
            .Include(t => t.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            travelerQuery = travelerQuery.Where(t =>
                t.FullName.Contains(search) ||
                (t.PassportNo != null && t.PassportNo.Contains(search)) ||
                (t.Customer != null && t.Customer.CustomerName.Contains(search)));
        }

        var travelers = await travelerQuery
            .OrderBy(t => t.FullName)
            .Select(t => new TravelerDocumentListItem
            {
                TravelerId = t.TravelerId,
                FullName = t.FullName,
                CustomerName = t.Customer != null ? t.Customer.CustomerName : null,
                PassportNo = t.PassportNo,
                DocumentCount = t.TravelerDocuments.Count
            })
            .ToListAsync(cancellationToken);

        Traveler? selected = null;
        IReadOnlyList<TravelerDocument> documents = Array.Empty<TravelerDocument>();
        if (travelerId.HasValue)
        {
            selected = await _db.Travelers.AsNoTracking()
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.TravelerId == travelerId.Value, cancellationToken);
            if (selected != null)
            {
                documents = await _db.TravelerDocuments.AsNoTracking()
                    .Where(d => d.TravelerId == travelerId.Value)
                    .OrderByDescending(d => d.UploadedDate)
                    .ToListAsync(cancellationToken);
            }
        }

        return new TravelerDocumentsPageViewModel
        {
            SelectedTravelerId = travelerId,
            Search = search,
            Travelers = travelers,
            SelectedTraveler = selected,
            Documents = documents,
            Upload = new TravelerDocumentInputModel
            {
                TravelerId = travelerId ?? 0,
                DocumentType = "Passport",
                DocumentNumber = selected?.PassportNo
            }
        };
    }

    public async Task DeleteDocumentAsync(int documentId, CancellationToken cancellationToken = default)
    {
        var doc = await _db.TravelerDocuments.FindAsync([documentId], cancellationToken);
        if (doc is null) return;
        _db.TravelerDocuments.Remove(doc);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
