using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("OrderDetail")]
public partial class OrderDetail
{
    [Key]
    [Column("OrderDetailID")]
    public int OrderDetailId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    public double Quantity { get; set; }

    public string? MainImage { get; set; }

    public string? ProductName { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(20, 8)")]
    public decimal? DiscountValue { get; set; }

    [Column(TypeName = "decimal(21, 8)")]
    public decimal? FinalPrice { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? OrderNumber { get; set; }

    public double? SubTotal { get; set; }

    [ForeignKey("OrderNumber")]
    [InverseProperty("OrderDetails")]
    public virtual Order? OrderNumberNavigation { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    public virtual Product? Product { get; set; }
}
