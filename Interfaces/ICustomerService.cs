using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface ICustomerService
{
    Task<(List<Customer> Items, int Total)> GetPagedAsync(string? q, int page, int pageSize);
    Task<Customer?> GetByIdAsync(int id);
    Task<CustomerDetailsViewModel?> GetDetailsAsync(int id);
    Task<Customer> SaveAsync(Customer entity);
    Task SoftDeleteAsync(int id);
}
