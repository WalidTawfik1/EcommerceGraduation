using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

[Table("Customer")]
[Index("Email", Name = "IDX_Customer_Email")]
[Index("CustomerCode", Name = "UQ__Customer__066785210EEA4C72", IsUnique = true)]
[Index("Email", Name = "UQ__Customer__A9D105346050AA0D", IsUnique = true)]
public partial class Customer
{
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string UserType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Key]
    [StringLength(20)]
    public string CustomerCode { get; set; } = null!;

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [InverseProperty("ChangedByNavigation")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("CustomerCodeNavigation")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("CustomerCodeNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("CustomerCodeNavigation")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}
