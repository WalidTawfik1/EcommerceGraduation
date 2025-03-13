using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        //For Future Methods
        Task<bool> AddAsync(AddProductDTO productDTO);
        Task<bool> UpdateAsync(UpdateProductDTO productDTO);
        Task DeleteAsync (Product product);
        Task<IEnumerable<ProductDTO>> GetAllAsync(ProductParams productParams);

    }
}
