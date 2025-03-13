using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("AuditLog")]
public partial class AuditLog
{
    [Key]
    [Column("LogID")]
    public int LogId { get; set; }

    [StringLength(255)]
    public string? TableName { get; set; }

    [StringLength(50)]
    public string? Action { get; set; }

    [StringLength(20)]
    public string? ChangedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ChangeDate { get; set; }

    [ForeignKey("ChangedBy")]
    [InverseProperty("AuditLogs")]
    public virtual Customer? ChangedByNavigation { get; set; }
}
