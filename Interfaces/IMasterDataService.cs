using TourismHajj.Models.Entities;

namespace TourismHajj.Interfaces;

public interface IMasterDataService
{
    IQueryable<Country> GetCountriesQuery(string? search = null, bool? activeOnly = null);
    Task<Country?> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Country> CreateCountryAsync(Country entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateCountryAsync(Country entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteCountryAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<City> GetCitiesQuery(string? search = null, int? countryId = null, bool? activeOnly = null);
    Task<City?> GetCityByIdAsync(int id, bool includeCountry = false, CancellationToken cancellationToken = default);
    Task<City> CreateCityAsync(City entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateCityAsync(City entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteCityAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<Destination> GetDestinationsQuery(string? search = null, int? cityId = null, bool? activeOnly = null);
    Task<Destination?> GetDestinationByIdAsync(int id, bool includeCity = false, CancellationToken cancellationToken = default);
    Task<Destination> CreateDestinationAsync(Destination entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateDestinationAsync(Destination entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteDestinationAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<Service> GetServicesQuery(string? search = null, bool? activeOnly = null);
    Task<Service?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Service> CreateServiceAsync(Service entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateServiceAsync(Service entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteServiceAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<Employee> GetEmployeesQuery(string? search = null, bool? activeOnly = null);
    Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee> CreateEmployeeAsync(Employee entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateEmployeeAsync(Employee entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteEmployeeAsync(int id, CancellationToken cancellationToken = default);

    Task<IList<Country>> GetActiveCountriesListAsync(CancellationToken cancellationToken = default);
    Task<IList<City>> GetActiveCitiesListAsync(int? countryId = null, CancellationToken cancellationToken = default);
}