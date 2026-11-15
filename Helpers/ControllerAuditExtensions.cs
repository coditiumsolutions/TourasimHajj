using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Interfaces;

namespace TourismHajj.Helpers;

public static class ControllerAuditExtensions
{
    public static Task LogMasterDataAsync(
        this IAuditService auditService,
        Controller controller,
        string table,
        string recordId,
        string action)
    {
        var userId = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ip = controller.HttpContext.Connection.RemoteIpAddress?.ToString();
        return auditService.LogAsync(userId, "MasterData", table, recordId, action, ip: ip);
    }
}