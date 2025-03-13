using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Category")]
[Index("Name", Name = "UQ__Category__737584F6BA133EFE", IsUnique = true)]
public partial class Category
{
    [Key]
    [StringLength(10)]
    public string CategoryCode { get; set; } = null!;

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [InverseProperty("CategoryCodeNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("CategoryCodeNavigation")]
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
