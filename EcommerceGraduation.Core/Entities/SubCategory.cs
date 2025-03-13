using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("SubCategory")]
[Index("Name", Name = "UQ__SubCateg__737584F670BC1034", IsUnique = true)]
public partial class SubCategory
{
    [Key]
    [StringLength(10)]
    public string SubCategoryCode { get; set; } = null!;

    [StringLength(10)]
    public string CategoryCode { get; set; } = null!;

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [ForeignKey("CategoryCode")]
    [InverseProperty("SubCategories")]
    public virtual Category CategoryCodeNavigation { get; set; } = null!;

    [InverseProperty("SubCategoryCodeNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
