using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Shipping")]
[Index("TrackingNumber", Name = "IDX_Shipping_Tracking")]
[Index("TrackingNumber", Name = "UQ__Shipping__784DB3D9105751C4", IsUnique = true)]
public partial class Shipping
{
    [Key]
    [Column("ShippingID")]
    public int ShippingId { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    public ShippingMethod ShippingMethod { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal ShippingCost { get; set; }

    [StringLength(100)]
    public string? Carrier { get; set; } = "DHL";

    [StringLength(50)]
    public string? TrackingNumber { get; set; }

    public Status ShippingStatus { get; set; } = Status.Pending;

    public DateOnly? EstimatedDeliveryDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? OrderNumber { get; set; }

    [JsonIgnore]
    [ForeignKey("OrderNumber")]
    [InverseProperty("Shippings")]
    public virtual Order? OrderNumberNavigation { get; set; }
}
