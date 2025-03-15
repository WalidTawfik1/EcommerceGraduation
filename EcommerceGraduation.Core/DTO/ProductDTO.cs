using EcommerceGraduation.Core.Sharing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
 
  public record ProductDTO
  {
      public string Name { get; set; }

      public string Description { get; set; }

      public decimal Price { get; set; }

      public virtual List<PhotoDTO>? Photos { get; set; }

      public string CategoryName { get; set; }

      public string SubCategoryName { get; set; }

      public string BrandName { get; set; }

      public int StockQuantity { get; set; }


    }

    public record PhotoDTO
  {
      public string ImageURL { get; set; }
      public int ProductID { get; set; }
  }

    public record AddProductDTO
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string CategoryCode { get; set; }

        public string SubCategoryCode { get; set; }

        public string BrandCode { get; set; }

        public int StockQuantity { get; set; }

        public IFormFileCollection Photo { get; set; }
    }

    public record UpdateProductDTO : AddProductDTO
  {
      public int ProductId { get; set; }
  }
    
}
