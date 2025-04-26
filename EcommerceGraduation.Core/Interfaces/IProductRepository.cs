using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product,int>
    {
        Task<bool> AddAsync(AddProductDTO productDTO);
        Task<bool> UpdateAsync(UpdateProductDTO productDTO);
        Task DeleteAsync(Product product);
        Task<IEnumerable<ProductDTO>> GetAllAsync(ProductParams productParams);
        Task<IEnumerable<ProductDTO>> GetAllNoPaginateAsync(ProductParams2 productParams2);
    }
}
