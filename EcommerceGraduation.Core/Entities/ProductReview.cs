using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("ProductReview")]
public partial class ProductReview
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }

    public string? ReviewText { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; } = DateTime.Now;

    [StringLength(20)]
    public string CustomerCode { get; set; } = null!;

    [ForeignKey("CustomerCode")]
    [InverseProperty("ProductReviews")]
    public virtual Customer CustomerCodeNavigation { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product? Product { get; set; }
}
