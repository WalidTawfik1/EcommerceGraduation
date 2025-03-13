using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetBasketAsync(string CartId);
        Task<Cart> UpdateBasketAsync(Cart cart);
        Task<bool> DeleteBasketAsync(string CartId);
    }
}
