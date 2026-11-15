using TourismHajj.Data;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;

namespace TourismHajj.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;

    public AuditService(ApplicationDbContext db) => _db = db;

    public async Task LogAsync(string? userId, string module, string table, string? recordId, string action, string? oldValues = null, string? newValues = null, string? ip = null)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ModuleName = module,
            TableName = table,
            RecordId = recordId,
            ActionType = action,
            OldValues = oldValues,
            NewValues = newValues,
            ActionDate = DateTime.Now,
            Ipaddress = ip
        });
        await _db.SaveChangesAsync();
    }
}
