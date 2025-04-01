using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceGraduation.Core.Sharing;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Product")]
[Index("Barcode", Name = "IDX_Product_Barcode")]
[Index("Barcode", Name = "UQ__Product__177800D3C1F80B60", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [StringLength(50)]
    public string? Barcode { get; set; } = GenerateCode.GetBarcode();

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(30)]
    public string? Weight { get; set; }

    [StringLength(10)]
    public string CategoryCode { get; set; } = null!;

    [StringLength(10)]
    public string SubCategoryCode { get; set; } = null!;

    [StringLength(20)]
    public string BrandCode { get; set; } = null!;

    public double? Rating { get; set; }

    [ForeignKey("BrandCode")]
    [InverseProperty("Products")]
    public virtual Brand BrandCodeNavigation { get; set; } = null!;

    [ForeignKey("CategoryCode")]
    [InverseProperty("Products")]
    public virtual Category CategoryCodeNavigation { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [ForeignKey("SubCategoryCode")]
    [InverseProperty("Products")]
    public virtual SubCategory SubCategoryCodeNavigation { get; set; } = null!;
    public Product()
    {
        Barcode = GenerateCode.GetBarcode();

    }
}
