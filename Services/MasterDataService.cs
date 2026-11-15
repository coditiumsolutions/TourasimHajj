using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Services;

public class MasterDataService : IMasterDataService
{
    private readonly ApplicationDbContext _context;

    public MasterDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Country> GetCountriesQuery(string? search = null, bool? activeOnly = null)
    {
        var query = _context.Countries.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.CountryName.Contains(search) ||
                (c.CountryCode != null && c.CountryCode.Contains(search)));
        }
        if (activeOnly == true)
        {
            query = query.Where(c => c.IsActive != false);
        }
        return query.OrderBy(c => c.CountryName);
    }

    public async Task<Country?> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Countries.FindAsync([id], cancellationToken);

    public async Task<Country> CreateCountryAsync(Country entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive ??= true;
        _context.Countries.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateCountryAsync(Country entity, CancellationToken cancellationToken = default)
    {
        _context.Countries.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> SoftDeleteCountryAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetCountryByIdAsync(id, cancellationToken);
        if (entity == null) return false;
        entity.IsActive = false;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public IQueryable<City> GetCitiesQuery(string? search = null, int? countryId = null, bool? activeOnly = null)
    {
        var query = _context.Cities.Include(c => c.Country).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.CityName.Contains(search));
        }
        if (countryId.HasValue)
        {
            query = query.Where(c => c.CountryId == countryId.Value);
        }
        if (activeOnly == true)
        {
            query = query.Where(c => c.IsActive != false);
        }
        return query.OrderBy(c => c.CityName);
    }

    public async Task<City?> GetCityByIdAsync(int id, bool includeCountry = false, CancellationToken cancellationToken = default)
    {
        if (!includeCountry)
        {
            return await _context.Cities.FindAsync([id], cancellationToken);
        }
        return await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.CityId == id, cancellationToken);
    }

    public async Task<City> CreateCityAsync(City entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive ??= true;
        _context.Cities.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateCityAsync(City entity, CancellationToken cancellationToken = default)
    {
        _context.Cities.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> SoftDeleteCityAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetCityByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;
        entity.IsActive = false;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public IQueryable<Destination> GetDestinationsQuery(string? search = null, int? cityId = null, bool? activeOnly = null)
    {
        var query = _context.Destinations.Include(d => d.City).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(d => d.DestinationName.Contains(search));
        }
        if (cityId.HasValue)
        {
            query = query.Where(d => d.CityId == cityId.Value);
        }
        if (activeOnly == true)
        {
            query = query.Where(d => d.IsActive != false);
        }
        return query.OrderBy(d => d.DestinationName);
    }

    public async Task<Destination?> GetDestinationByIdAsync(int id, bool includeCity = false, CancellationToken cancellationToken = default)
    {
        if (!includeCity)
        {
            return await _context.Destinations.FindAsync([id], cancellationToken);
        }
        return await _context.Destinations.Include(d => d.City).ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(d => d.DestinationId == id, cancellationToken);
    }

    public async Task<Destination> CreateDestinationAsync(Destination entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive ??= true;
        entity.IsFeatured ??= false;
        _context.Destinations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateDestinationAsync(Destination entity, CancellationToken cancellationToken = default)
    {
        _context.Destinations.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> SoftDeleteDestinationAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetDestinationByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;
        entity.IsActive = false;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public IQueryable<Service> GetServicesQuery(string? search = null, bool? activeOnly = null)
    {
        var query = _context.Services.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.ServiceName.Contains(search) ||
                (s.ServiceCategory != null && s.ServiceCategory.Contains(search)));
        }
        if (activeOnly == true)
        {
            query = query.Where(s => s.IsActive != false);
        }
        return query.OrderBy(s => s.ServiceName);
    }

    public async Task<Service?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Services.FindAsync([id], cancellationToken);

    public async Task<Service> CreateServiceAsync(Service entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive ??= true;
        _context.Services.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateServiceAsync(Service entity, CancellationToken cancellationToken = default)
    {
        _context.Services.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> SoftDeleteServiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetServiceByIdAsync(id, cancellationToken);
        if (entity == null) return false;
        entity.IsActive = false;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public IQueryable<Employee> GetEmployeesQuery(string? search = null, bool? activeOnly = null)
    {
        var query = _context.Employees.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.EmployeeName.Contains(search) ||
                (e.Designation != null && e.Designation.Contains(search)) ||
                (e.Email != null && e.Email.Contains(search)));
        }
        if (activeOnly == true)
        {
            query = query.Where(e => e.IsActive != false);
        }
        return query.OrderBy(e => e.EmployeeName);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Employees.FindAsync([id], cancellationToken);

    public async Task<Employee> CreateEmployeeAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive ??= true;
        entity.Salary ??= 0m;
        _context.Employees.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateEmployeeAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> SoftDeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEmployeeByIdAsync(id, cancellationToken);
        if (entity == null) return false;
        entity.IsActive = false;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IList<Country>> GetActiveCountriesListAsync(CancellationToken cancellationToken = default)
        => await GetCountriesQuery(activeOnly: true).ToListAsync(cancellationToken);

    public async Task<IList<City>> GetActiveCitiesListAsync(int? countryId = null, CancellationToken cancellationToken = default)
        => await GetCitiesQuery(countryId: countryId, activeOnly: true).ToListAsync(cancellationToken);
}