using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IPublicSiteService
{
    Task<PublicHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default);
    Task<List<PublicPackageCardViewModel>> SearchPackagesAsync(PackageSearchViewModel search, CancellationToken cancellationToken = default);
    Task<PublicPackageDetailsViewModel?> GetPackageDetailsAsync(int packageId, CancellationToken cancellationToken = default);
    Task SubmitContactAsync(ContactFormViewModel model, CancellationToken cancellationToken = default);
    Task<List<Destination>> GetActiveDestinationsAsync(CancellationToken cancellationToken = default);
}
