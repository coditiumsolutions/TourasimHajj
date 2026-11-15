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

    public async Task<Customer> SaveAsync(Customer entity)
    {
        if (entity.CustomerId == 0)
        {
            entity.CreatedDate = DateTime.Now;
            entity.IsActive ??= true;
            _db.Customers.Add(entity);
        }
        else
        {
            _db.Customers.Update(entity);
        }
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task SoftDeleteAsync(int id)
    {
        var e = await _db.Customers.FindAsync(id);
        if (e == null) return;
        e.IsActive = false;
        await _db.SaveChangesAsync();
    }
}
