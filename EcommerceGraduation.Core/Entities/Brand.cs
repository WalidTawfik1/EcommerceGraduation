using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Brand")]
[Index("BrandName", Name = "UQ__Brand__2206CE9B6FC305AD", IsUnique = true)]
[Index("BrandCode", Name = "UQ__Brand__44292CC74BAD8DC1", IsUnique = true)]
public partial class Brand
{
    [StringLength(255)]
    public string BrandName { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string BrandCode { get; set; } = null!;

    [InverseProperty("BrandCodeNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
