
using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IWishlistRepository
    {
        Task<bool> AddToWishlist(string whishlistId, int productId);
        Task<bool> RemoveFromWishlist(string whislistId, int productId);
        Task<Wishlist> GetWishlist(string whislistId);
        Task<bool> ClearWishlist(string whishlistId);
    }
}
