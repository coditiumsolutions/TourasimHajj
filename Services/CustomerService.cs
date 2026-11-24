using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _db;
    public CustomerService(ApplicationDbContext db) => _db = db;

    public async Task<(List<Customer> Items, int Total)> GetPagedAsync(string? q, int page, int pageSize)
    {
        var query = _db.Customers.Include(c => c.City).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c =>
                c.CustomerName.Contains(q) ||
                (c.TrackingNo != null && c.TrackingNo.Contains(q)) ||
                (c.MobileNo != null && c.MobileNo.Contains(q)) ||
                (c.Cnic != null && c.Cnic.Contains(q)) ||
                (c.Email != null && c.Email.Contains(q)));
        }
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CustomerId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Customer?> GetByIdAsync(int id) =>
        _db.Customers.Include(c => c.City).FirstOrDefaultAsync(c => c.CustomerId == id);

    public async Task<CustomerInputModel?> GetFormAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == id, cancellationToken);
        if (customer is null) return null;

        var members = await _db.Travelers.AsNoTracking()
            .Where(t => t.CustomerId == id)
            .OrderBy(t => t.TravelerId)
            .Select(t => new CustomerGroupMemberInputModel
            {
                TravelerId = t.TravelerId,
                FullName = t.FullName,
                Relationship = t.Relationship,
                PassportNo = t.PassportNo,
                MobileNo = t.MobileNo,
                Gender = t.Gender
            })
            .ToListAsync(cancellationToken);

        return MapToInput(customer, members);
    }

    public async Task<CustomerDetailsViewModel?> GetDetailsAsync(int id)
    {
        var customer = await _db.Customers.Include(c => c.City).FirstOrDefaultAsync(c => c.CustomerId == id);
        if (customer == null) return null;

        var travelers = await _db.Travelers.Where(t => t.CustomerId == id).ToListAsync();
        var bookings = await _db.Bookings.Include(b => b.Package)
            .Where(b => b.CustomerId == id).OrderByDescending(b => b.BookingDate).ToListAsync();
        var bookingIds = bookings.Select(b => b.BookingId).ToList();
        var payments = await _db.Payments.Where(p => bookingIds.Contains(p.BookingId))
            .OrderByDescending(p => p.PaymentDate).ToListAsync();

        var bookingRows = bookings.Select(b => new BookingSummaryRow
        {
            BookingId = b.BookingId,
            BookingNo = b.BookingNo,
            PackageName = b.Package?.PackageName,
            BookingDate = b.BookingDate,
            BookingStatus = b.BookingStatus,
            NetAmount = b.NetAmount,
            PaidAmount = b.PaidAmount,
            BalanceAmount = b.BalanceAmount
        }).ToList();
        var paymentRows = payments.Select(p => new PaymentRow
        {
            PaymentId = p.PaymentId,
            BookingNo = bookings.FirstOrDefault(b => b.BookingId == p.BookingId)?.BookingNo ?? "",
            PaymentDate = p.PaymentDate,
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount
        }).ToList();

        return new CustomerDetailsViewModel
        {
            Customer = customer,
            Travelers = travelers,
            Bookings = bookingRows,
            Payments = paymentRows,
            TotalOutstanding = bookings.Where(b => b.BookingStatus != "Cancelled").Sum(b => b.BalanceAmount ?? 0)
        };
    }

    public async Task<(bool Success, string? Error, int CustomerId)> SaveWithGroupAsync(
        CustomerInputModel model,
        CancellationToken cancellationToken = default)
    {
        var members = (model.Members ?? new())
            .Where(m => !string.IsNullOrWhiteSpace(m.FullName))
            .ToList();

        Customer entity;
        if (model.CustomerId == 0)
        {
            entity = new Customer { CreatedDate = DateTime.Now };
            _db.Customers.Add(entity);
        }
        else
        {
            entity = await _db.Customers
                .Include(c => c.Travelers)
                .FirstOrDefaultAsync(c => c.CustomerId == model.CustomerId, cancellationToken);
            if (entity is null)
                return (false, "Customer not found.", 0);
        }

        entity.CustomerName = model.CustomerName.Trim();
        entity.FatherName = model.FatherName;
        entity.MobileNo = model.MobileNo;
        entity.WhatsAppNo = model.WhatsAppNo;
        entity.Email = model.Email;
        entity.Cnic = model.Cnic;
        entity.Address = model.Address;
        entity.CityId = model.CityId;
        entity.CustomerType = string.IsNullOrWhiteSpace(model.CustomerType) ? "Individual" : model.CustomerType.Trim();
        entity.GroupType = string.IsNullOrWhiteSpace(model.GroupType) ? null : model.GroupType.Trim();
        entity.IsActive = model.IsActive;
        entity.TrackingNo = string.IsNullOrWhiteSpace(model.TrackingNo) ? null : model.TrackingNo.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(entity.TrackingNo))
        {
            entity.TrackingNo = $"CUST-{entity.CustomerId:D5}";
            await _db.SaveChangesAsync(cancellationToken);
        }

        var keepIds = new HashSet<int>();
        foreach (var member in members)
        {
            Traveler traveler;
            if (member.TravelerId > 0)
            {
                traveler = entity.Travelers.FirstOrDefault(t => t.TravelerId == member.TravelerId)
                    ?? await _db.Travelers.FirstOrDefaultAsync(t => t.TravelerId == member.TravelerId && t.CustomerId == entity.CustomerId, cancellationToken)
                    ?? new Traveler { CustomerId = entity.CustomerId, CreatedDate = DateTime.Now };
                if (traveler.TravelerId == 0)
                    _db.Travelers.Add(traveler);
            }
            else
            {
                traveler = new Traveler { CustomerId = entity.CustomerId, CreatedDate = DateTime.Now };
                _db.Travelers.Add(traveler);
            }

            traveler.FullName = member.FullName!.Trim();
            traveler.Relationship = string.IsNullOrWhiteSpace(member.Relationship) ? null : member.Relationship.Trim();
            traveler.PassportNo = string.IsNullOrWhiteSpace(member.PassportNo) ? null : member.PassportNo.Trim();
            traveler.MobileNo = string.IsNullOrWhiteSpace(member.MobileNo) ? null : member.MobileNo.Trim();
            traveler.Gender = string.IsNullOrWhiteSpace(member.Gender) ? null : member.Gender.Trim();

            await _db.SaveChangesAsync(cancellationToken);
            keepIds.Add(traveler.TravelerId);
        }

        var existing = await _db.Travelers
            .Where(t => t.CustomerId == entity.CustomerId)
            .ToListAsync(cancellationToken);

        foreach (var traveler in existing.Where(t => !keepIds.Contains(t.TravelerId)))
        {
            var inBooking = await _db.BookingTravelers.AnyAsync(bt => bt.TravelerId == traveler.TravelerId, cancellationToken);
            if (!inBooking)
                _db.Travelers.Remove(traveler);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return (true, null, entity.CustomerId);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var e = await _db.Customers.FindAsync(id);
        if (e == null) return;
        e.IsActive = false;
        await _db.SaveChangesAsync();
    }

    private static CustomerInputModel MapToInput(Customer customer, List<CustomerGroupMemberInputModel> members) => new()
    {
        CustomerId = customer.CustomerId,
        CustomerName = customer.CustomerName,
        FatherName = customer.FatherName,
        MobileNo = customer.MobileNo,
        WhatsAppNo = customer.WhatsAppNo,
        Email = customer.Email,
        Cnic = customer.Cnic,
        Address = customer.Address,
        CityId = customer.CityId,
        CustomerType = customer.CustomerType ?? "Individual",
        TrackingNo = customer.TrackingNo,
        GroupType = customer.GroupType,
        IsActive = customer.IsActive != false,
        CreatedDate = customer.CreatedDate,
        Members = members.Count > 0 ? members : new List<CustomerGroupMemberInputModel> { new() }
    };
}
