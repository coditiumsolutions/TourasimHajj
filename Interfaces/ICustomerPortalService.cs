using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface ICustomerPortalService
{
    Task<Customer?> GetCustomerByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<CustomerPortalDashboardViewModel> GetDashboardAsync(string userId, CancellationToken cancellationToken = default);
    Task<CustomerProfileViewModel?> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<(bool Ok, string Message)> UpdateProfileAsync(string userId, CustomerProfileViewModel model, CancellationToken cancellationToken = default);
    Task<List<Traveler>> GetTravelersAsync(string userId, CancellationToken cancellationToken = default);
    Task<(bool Ok, string Message)> SaveTravelerAsync(string userId, Traveler traveler, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingsAsync(string userId, string? typeFilter = null, CancellationToken cancellationToken = default);
    Task<BookingDetailsViewModel?> GetBookingDetailsAsync(string userId, int bookingId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetPaymentsAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<TravelerDocument>> GetDocumentsAsync(string userId, CancellationToken cancellationToken = default);
    Task<(bool Ok, string Message, int? BookingId)> CreateBookingAsync(string userId, CustomerBookingRequestViewModel model, CancellationToken cancellationToken = default);
    Task<CustomerBookingRequestViewModel?> GetBookingRequestAsync(string userId, int packageId, CancellationToken cancellationToken = default);
}
