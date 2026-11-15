namespace TourismHajj.Interfaces;

public interface IAuditService
{
    Task LogAsync(string? userId, string module, string table, string? recordId, string action, string? oldValues = null, string? newValues = null, string? ip = null);
}
