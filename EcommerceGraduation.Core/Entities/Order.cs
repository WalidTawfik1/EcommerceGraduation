using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Index("OrderDate", Name = "IDX_Order_Date")]
public partial class Order
{
    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [StringLength(50)]
    public string? OrderStatus { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(20)]
    public string CustomerCode { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string OrderNumber { get; set; } = null!;

    [ForeignKey("CustomerCode")]
    [InverseProperty("Orders")]
    public virtual Customer CustomerCodeNavigation { get; set; } = null!;

    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}
