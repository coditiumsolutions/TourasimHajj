using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IBookingService
{
    Task<(List<Booking> Items, int Total)> GetPagedAsync(string? q, string? type, string? status, int page, int pageSize);
    Task<Booking?> GetByIdAsync(int id);
    Task<BookingDetailsViewModel?> GetDetailsAsync(int id);
    Task<(bool Ok, string Message, int? BookingId)> CreateBookingAsync(BookingCreateViewModel model, string? userId);
    Task<(bool Ok, string Message)> CancelAsync(int bookingId, string? userId);
    Task RecalculatePaymentTotalsAsync(int bookingId);
}
