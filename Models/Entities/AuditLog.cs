using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourismHajj.Models.Entities;

public partial class AuditLog
{
    [Key]
    [Column("AuditLogID")]
    public long AuditLogId { get; set; }

    [Column("UserID")]
    [StringLength(450)]
    public string? UserId { get; set; }

    [StringLength(100)]
    public string? ModuleName { get; set; }

    [StringLength(100)]
    public string? TableName { get; set; }

    [Column("RecordID")]
    [StringLength(100)]
    public string? RecordId { get; set; }

    [StringLength(50)]
    public string? ActionType { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime? ActionDate { get; set; }

    [Column("IPAddress")]
    [StringLength(50)]
    public string? Ipaddress { get; set; }
}
