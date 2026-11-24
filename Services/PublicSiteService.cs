using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class PublicSiteService : IPublicSiteService
{
    private readonly ApplicationDbContext _db;

    private static readonly string DefaultCover =
        "https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&w=1200&q=80";

    private static readonly string DubaiCover =
        "https://images.unsplash.com/photo-1512453979798-5ea266f8880c?auto=format&fit=crop&w=1200&q=80";

    private static readonly string UmrahCover =
        "https://images.unsplash.com/photo-1591604129939-f1efa4d9f7fa?auto=format&fit=crop&w=1200&q=80";

    private static readonly string HajjCover =
        "https://images.unsplash.com/photo-1564769625905-50e93615e769?auto=format&fit=crop&w=1200&q=80";

    private static readonly string ZiaratCover =
        "https://images.unsplash.com/photo-1564760055775-d63b17a55c44?auto=format&fit=crop&w=1200&q=80";

    private static readonly string PakistanCover =
        "https://images.unsplash.com/photo-1589802829985-817e51171b92?auto=format&fit=crop&w=1200&q=80";

    private static string ResolveCoverImage(TravelPackage package, string? destinationImageUrl)
    {
        if (!string.IsNullOrWhiteSpace(package.CoverImageUrl))
            return package.CoverImageUrl!;

        if (!string.IsNullOrWhiteSpace(destinationImageUrl))
            return destinationImageUrl!;

        var name = package.PackageName ?? string.Empty;
        if (name.Contains("Dubai", StringComparison.OrdinalIgnoreCase))
            return DubaiCover;
        if (name.Contains("Pakistan", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Northern", StringComparison.OrdinalIgnoreCase))
            return PakistanCover;

        return package.PackageType switch
        {
            "Hajj" => HajjCover,
            "Umrah" => UmrahCover,
            "Ziarat" => ZiaratCover,
            "Domestic" => PakistanCover,
            "International" => DubaiCover,
            _ => DefaultCover
        };
    }

    public PublicSiteService(ApplicationDbContext db) => _db = db;

    public async Task<PublicHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default)
    {
        var destinations = await _db.Destinations
            .Include(d => d.City)
            .Where(d => d.IsActive != false)
            .OrderByDescending(d => d.IsFeatured == true)
            .ThenBy(d => d.DestinationName)
            .Take(12)
            .ToListAsync(cancellationToken);

        return new PublicHomeViewModel
        {
            Search = new PackageSearchViewModel
            {
                DestinationOptions = destinations
            },
            FeaturedPackages = await MapPackagesAsync(
                await QueryPackages().Where(p => p.IsFeatured == true).Take(8).ToListAsync(cancellationToken),
                cancellationToken),
            HajjPackages = await MapPackagesAsync(
                await QueryPackages().Where(p => p.PackageType == "Hajj").Take(6).ToListAsync(cancellationToken),
                cancellationToken),
            UmrahPackages = await MapPackagesAsync(
                await QueryPackages().Where(p => p.PackageType == "Umrah").Take(6).ToListAsync(cancellationToken),
                cancellationToken),
            Destinations = destinations.Select(d => new PublicDestinationCardViewModel
            {
                DestinationId = d.DestinationId,
                DestinationName = d.DestinationName,
                Description = d.Description,
                ImageUrl = string.IsNullOrWhiteSpace(d.ImageUrl) ? DefaultCover : d.ImageUrl,
                CityName = d.City?.CityName
            }).ToList()
        };
    }

    public async Task<List<PublicPackageCardViewModel>> SearchPackagesAsync(PackageSearchViewModel search, CancellationToken cancellationToken = default)
    {
        var query = QueryPackages();

        if (!string.IsNullOrWhiteSpace(search.PackageType) && search.PackageType != "All")
            query = query.Where(p => p.PackageType == search.PackageType);

        if (search.DestinationId.HasValue)
            query = query.Where(p => p.PackageDestinations.Any(pd => pd.DestinationId == search.DestinationId));

        if (search.DepartureDate.HasValue)
            query = query.Where(p => p.PackageDepartures.Any(d => d.DepartureDate >= search.DepartureDate));

        if (search.Travelers > 0)
            query = query.Where(p =>
                p.PackageDepartures.Any(d => (d.AvailableSeats ?? 0) >= search.Travelers) ||
                (p.MaximumPersons == null || p.MaximumPersons >= search.Travelers));

        var packages = await query.Take(24).ToListAsync(cancellationToken);
        return await MapPackagesAsync(packages, cancellationToken);
    }

    public async Task<PublicPackageDetailsViewModel?> GetPackageDetailsAsync(int packageId, CancellationToken cancellationToken = default)
    {
        var package = await _db.TravelPackages
            .Include(p => p.PackageDestinations).ThenInclude(pd => pd.Destination).ThenInclude(d => d.City)
            .Include(p => p.PackageItineraries).ThenInclude(i => i.Destination)
            .Include(p => p.PackageServices).ThenInclude(ps => ps.Service)
            .Include(p => p.PackageDepartures)
            .FirstOrDefaultAsync(p => p.PackageId == packageId && p.IsActive != false, cancellationToken);

        if (package == null) return null;

        var cityIds = package.PackageDestinations
            .Select(pd => pd.Destination?.CityId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var hotels = cityIds.Count == 0
            ? new List<Hotel>()
            : await _db.Hotels.Include(h => h.City)
                .Where(h => h.IsActive != false && cityIds.Contains(h.CityId))
                .Take(6)
                .ToListAsync(cancellationToken);

        var destImage = package.PackageDestinations
            .Select(pd => pd.Destination?.ImageUrl)
            .FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));

        return new PublicPackageDetailsViewModel
        {
            Package = package,
            CoverImageUrl = ResolveCoverImage(package, destImage),
            DestinationSummary = string.Join(", ",
                package.PackageDestinations
                    .OrderBy(pd => pd.SequenceNo)
                    .Select(pd => pd.Destination?.DestinationName)
                    .Where(n => !string.IsNullOrWhiteSpace(n))),
            Itineraries = package.PackageItineraries.OrderBy(i => i.DayNumber).ToList(),
            IncludedServices = package.PackageServices.Where(s => s.IsIncluded != false).ToList(),
            ExcludedServices = package.PackageServices.Where(s => s.IsIncluded == false).ToList(),
            Departures = package.PackageDepartures
                .Where(d => d.DepartureDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(d => d.DepartureDate)
                .ToList(),
            Hotels = hotels
        };
    }

    public async Task SubmitContactAsync(ContactFormViewModel model, CancellationToken cancellationToken = default)
    {
        _db.ContactInquiries.Add(new ContactInquiry
        {
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone,
            Subject = model.Subject,
            Message = model.Message.Trim(),
            CreatedDate = DateTime.UtcNow,
            IsRead = false
        });
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Destination>> GetActiveDestinationsAsync(CancellationToken cancellationToken = default) =>
        _db.Destinations.Include(d => d.City)
            .Where(d => d.IsActive != false)
            .OrderBy(d => d.DestinationName)
            .ToListAsync(cancellationToken);

    public async Task<List<PublicHotelCardViewModel>> GetPublicHotelsAsync(CancellationToken cancellationToken = default)
    {
        var packages = await _db.HotelPackages.AsNoTracking()
            .Where(h => h.IsActive)
            .OrderBy(h => h.SortOrder)
            .ThenByDescending(h => h.IsFeatured)
            .ThenByDescending(h => h.StarRating)
            .ThenBy(h => h.HotelName)
            .Select(h => new PublicHotelCardViewModel
            {
                HotelId = h.HotelPackageId,
                HotelName = h.HotelName,
                Description = h.Description,
                CityName = h.CityName,
                StarRating = h.StarRating,
                Address = h.Address,
                DistanceFromHaramKm = h.DistanceFromHaramKm,
                DistanceFromNabawiKm = h.DistanceFromNabawiKm,
                ContactNo = h.ContactNo,
                CoverImageUrl = h.CoverImageUrl,
                PricePerNight = h.PricePerNight,
                CurrencyCode = h.CurrencyCode,
                DurationNights = h.DurationNights,
                IsFeatured = h.IsFeatured
            })
            .ToListAsync(cancellationToken);

        if (packages.Count > 0)
            return packages;

        // Fallback to inventory hotels when no public hotel packages exist yet.
        return await _db.Hotels.AsNoTracking()
            .Include(h => h.City)
            .Where(h => h.IsActive != false)
            .OrderByDescending(h => h.StarRating)
            .ThenBy(h => h.HotelName)
            .Select(h => new PublicHotelCardViewModel
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                CityName = h.City.CityName,
                StarRating = h.StarRating,
                Address = h.Address,
                DistanceFromHaramKm = h.DistanceFromHaramKm,
                DistanceFromNabawiKm = h.DistanceFromNabawiKm,
                ContactNo = h.ContactNo
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PublicGalleryCardViewModel>> GetPublicGalleryAsync(CancellationToken cancellationToken = default)
    {
        return await _db.GalleryItems.AsNoTracking()
            .Where(g => g.IsActive)
            .OrderBy(g => g.SortOrder)
            .ThenByDescending(g => g.GalleryItemId)
            .Select(g => new PublicGalleryCardViewModel
            {
                GalleryItemId = g.GalleryItemId,
                Title = g.Title,
                Description = g.Description,
                ImageUrl = g.ImageUrl
            })
            .ToListAsync(cancellationToken);
    }

    private IQueryable<TravelPackage> QueryPackages() =>
        _db.TravelPackages
            .Include(p => p.PackageDestinations).ThenInclude(pd => pd.Destination)
            .Include(p => p.PackageDepartures)
            .Where(p => p.IsActive != false)
            .OrderByDescending(p => p.IsFeatured == true)
            .ThenBy(p => p.PackageName);

    private static Task<List<PublicPackageCardViewModel>> MapPackagesAsync(
        List<TravelPackage> packages,
        CancellationToken cancellationToken)
    {
        var cards = packages.Select(p =>
        {
            var dest = p.PackageDestinations
                .OrderBy(pd => pd.SequenceNo)
                .Select(pd => pd.Destination)
                .FirstOrDefault();
            var nextDep = p.PackageDepartures
                .Where(d => d.DepartureDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(d => d.DepartureDate)
                .FirstOrDefault();

            return new PublicPackageCardViewModel
            {
                PackageId = p.PackageId,
                PackageName = p.PackageName,
                PackageType = p.PackageType,
                Description = p.Description,
                DestinationName = dest?.DestinationName,
                DurationDays = p.DurationDays,
                DurationNights = p.DurationNights,
                AdultPrice = p.AdultPrice,
                ChildPrice = p.ChildPrice,
                InfantPrice = p.InfantPrice,
                CurrencyCode = p.CurrencyCode ?? "PKR",
                CoverImageUrl = ResolveCoverImage(p, dest?.ImageUrl),
                NextDepartureDate = nextDep?.DepartureDate,
                AccommodationSummary = p.PackageType is "Hajj" or "Umrah"
                    ? "Makkah & Madinah hotels arranged"
                    : "Selected hotels included"
            };
        }).ToList();

        return Task.FromResult(cards);
    }
}
