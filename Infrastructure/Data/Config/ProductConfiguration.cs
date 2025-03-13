using Ecom.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
    class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(p => p.NewPrice)
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.OldPrice)
             .HasColumnType("decimal(18,2)");
            builder.HasData(new Product { Id = 1, Name = "test", Description = "test ", NewPrice = 1000, CategoryId = 1 });
        }
    }
}
