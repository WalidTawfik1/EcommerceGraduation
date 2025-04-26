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
        Task<Cart> GetCartAsync(string CartId);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<Cart> AddToCartAsync(string cartId, int productId, int quantity);
        Task<bool> DeleteCartAsync(string CartId);
        Task<Cart> RemoveItemFromCartAsync(string cartId, int productId);
        Task<Cart> UpdateItemQuantityAsync(string cartId, int productId, int quantity);
    }

}
