using EcommerceGraduation.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IProductReview
    {
        Task<bool> AddRatingAsync(ProductReviewDTO reviewDTO, string CustomerCode);
        Task<IReadOnlyList<ReturnProductReviewDTO>> GetAllRatingForProductAsync(int productId);
    }
}
