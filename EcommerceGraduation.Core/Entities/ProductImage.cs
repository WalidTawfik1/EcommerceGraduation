using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("ProductImage")]
public partial class ProductImage
{
    [Key]
    [Column("ImageID")]
    public int ImageId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("ImageURL")]
    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product? Product { get; set; }
}
