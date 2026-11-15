using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<Supplier>> GetSuppliersAsync(string? typeFilter, bool includeInactive, CancellationToken cancellationToken = default);
    Task<Supplier?> GetSupplierByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SupplierInputModel> GetSupplierFormOptionsAsync(SupplierInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateSupplierAsync(SupplierInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateSupplierAsync(int id, SupplierInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteSupplierAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Hotel>> GetHotelsAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<Hotel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<HotelInputModel> GetHotelFormOptionsAsync(HotelInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateHotelAsync(HotelInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateHotelAsync(int id, HotelInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteHotelAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransportProvider>> GetTransportProvidersAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<TransportProvider?> GetTransportProviderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransportProviderInputModel> GetTransportProviderFormOptionsAsync(TransportProviderInputModel? model = null, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> CreateTransportProviderAsync(TransportProviderInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateTransportProviderAsync(int id, TransportProviderInputModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteTransportProviderAsync(int id, CancellationToken cancellationToken = default);
}