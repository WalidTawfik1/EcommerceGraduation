using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Payment")]
[Index("TransactionId", Name = "UQ__Payment__55433A4A975A59E3", IsUnique = true)]
public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [Column("TransactionID")]
    [StringLength(50)]
    public string? TransactionId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? OrderNumber { get; set; }

    [ForeignKey("OrderNumber")]
    [InverseProperty("Payments")]
    public virtual Order? OrderNumberNavigation { get; set; }
}
