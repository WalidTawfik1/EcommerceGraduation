using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly IDatabase _database;
        private readonly IProductRepository _productRepository;
        public WishlistRepository(IConnectionMultiplexer redis, IProductRepository productRepository)
        {
            _database = redis.GetDatabase();
            _productRepository = productRepository;
        }

        public async Task<bool> AddToWishlist(string whishlistId, int productId)
        {
            var wishlist = await GetWishlist(whishlistId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Id = whishlistId,
                };
            }
            var product = await _productRepository.GetByIdAsync(productId,p => p.ProductImages);
            if (product == null)
            {
                return false;
            }
            var existingItem = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                return false;
            }

            var photoUrl = product.ProductImages.FirstOrDefault()?.ImageUrl ?? "https://drive.google.com/open?id=1XPJh5f9DfFI_6ZZyaYGx04jbKHz9d6bV";

            wishlist.Items.Add(new Wishlist_Items
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Photo = photoUrl
            });
            var result = await _database.StringSetAsync(whishlistId, JsonSerializer.Serialize(wishlist));
            return result;
        }

        public Task<bool> ClearWishlist(string whishlistId)
        {
            return _database.KeyDeleteAsync(whishlistId);
        }

        public async Task<Wishlist> GetWishlist(string whislistId)
        {
            var result = await _database.StringGetAsync(whislistId);
            if (!string.IsNullOrEmpty(result))
            {
                return JsonSerializer.Deserialize<Wishlist>(result);
            }
            return null;
        }

        public Task<bool> RemoveFromWishlist(string userId, int productId)
        {
            var wishlist = GetWishlist(userId);
            if (wishlist == null)
            {
                return Task.FromResult(false);
            }
            var itemToRemove = wishlist.Result.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                wishlist.Result.Items.Remove(itemToRemove);
                return _database.StringSetAsync(userId, JsonSerializer.Serialize(wishlist.Result));
            }
            return Task.FromResult(false);
        }
    }
}
