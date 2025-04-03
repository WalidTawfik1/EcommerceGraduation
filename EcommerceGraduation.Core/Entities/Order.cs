using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Index("OrderDate", Name = "IDX_Order_Date")]
public partial class Order
{
    public Order()
    {
        OrderNumber = Guid.NewGuid().ToString("N")[..18];
    }
    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; } = DateTime.Now;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status OrderStatus { get; set; } = Status.Pending;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status PaymentStatus { get; set; } = Status.Pending;

    [StringLength(20)]
    public string CustomerCode { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string OrderNumber { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey("CustomerCode")]
    [InverseProperty("Orders")]
    public virtual Customer CustomerCodeNavigation { get; set; } = null!;

    [JsonIgnore]
    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [JsonIgnore]
    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [JsonIgnore]
    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [JsonIgnore]
    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}
