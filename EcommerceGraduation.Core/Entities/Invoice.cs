using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

public partial class Invoice
{
    [Key]
    [Column("InvoiceID")]
    public int InvoiceId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? OrderNumber { get; set; }

    [StringLength(20)]
    public string? CustomerCode { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalBeforeDiscount { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? ShippingValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? InvoiceDate { get; set; }

    [Column(TypeName = "decimal(20, 8)")]
    public decimal? TotalDiscount { get; set; }

    [Column(TypeName = "decimal(21, 8)")]
    public decimal? TotalAfterDiscount { get; set; }

    [Column(TypeName = "decimal(22, 8)")]
    public decimal? Total { get; set; }

    [ForeignKey("CustomerCode")]
    [InverseProperty("Invoices")]
    public virtual Customer? CustomerCodeNavigation { get; set; }

    [ForeignKey("OrderNumber")]
    [InverseProperty("Invoices")]
    public virtual Order? OrderNumberNavigation { get; set; }
}
