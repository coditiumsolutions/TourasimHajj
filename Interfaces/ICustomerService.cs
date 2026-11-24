using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface ICustomerService
{
    Task<(List<Customer> Items, int Total)> GetPagedAsync(string? q, int page, int pageSize);
    Task<Customer?> GetByIdAsync(int id);
    Task<CustomerInputModel?> GetFormAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDetailsViewModel?> GetDetailsAsync(int id);
    Task<(bool Success, string? Error, int CustomerId)> SaveWithGroupAsync(CustomerInputModel model, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(int id);
}
