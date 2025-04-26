using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Infrastructure.Data;
using NuGet.ContentModel;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;
        private readonly IProductRepository _productRepository;
        public CartRepository(IConnectionMultiplexer redis, IProductRepository productRepository)
        {
            _database = redis.GetDatabase();
            _productRepository = productRepository;
        }

        // In CartRepository.cs
        public async Task<Cart> AddToCartAsync(string cartId, int productId, int quantity)
        {
            if (string.IsNullOrEmpty(cartId) || productId <= 0 || quantity <= 0)
                return null;

            try
            {
                // Get existing cart or create new one
                var cart = await GetCartAsync(cartId);
                if (cart == null)
                    cart = new Cart(cartId);

                // Get product details from product repository with images explicitly included
                var product = await _productRepository.GetByIdAsync(productId, p => p.ProductImages);
                if (product == null)
                    return null;

                // Check if item already exists
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    // Update quantity if item exists
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var photoUrl = product.ProductImages.FirstOrDefault().ImageUrl;

                    // Add new item with product details
                    cart.Items.Add(new CartItem
                    {
                        ProductId = productId,
                        Name = product.Name,
                        Price = product.Price,
                        Photo = photoUrl,
                        Quantity = quantity
                    });
                }

                // Save updated cart
                return await UpdateCartAsync(cart);
            }
            catch (Exception ex)
            {
                // Consider adding logging here
                return null;
            }
        }




        public Task<bool> DeleteCartAsync(string CartId)
        {
            return _database.KeyDeleteAsync(CartId);
        }

        public async Task<Cart> GetCartAsync(string CartId)
        {
            var result = await _database.StringGetAsync(CartId);
            if (!string.IsNullOrEmpty(result))
            {
                return JsonSerializer.Deserialize<Cart>(result);
            }
            return null;
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            var _basket = await _database.StringSetAsync(cart.Id.ToString(),
                JsonSerializer.Serialize(cart),
                TimeSpan.FromDays(3));
            if (_basket)
            {
                return await GetCartAsync(cart.Id.ToString());
            }
            return null;
        }

        public async Task<Cart> RemoveItemFromCartAsync(string cartId, int productId)
        {
            if (string.IsNullOrEmpty(cartId) || productId <= 0)
                return null;

            try
            {
                // Get existing cart
                var cart = await GetCartAsync(cartId);
                if (cart == null || !cart.Items.Any())
                    return cart;

                // Remove the item if it exists
                int initialCount = cart.Items.Count;
                cart.Items.RemoveAll(item => item.ProductId == productId);

                // If item count didn't change, item wasn't found
                if (cart.Items.Count == initialCount)
                    return cart; // No changes made

                // Save the updated cart
                return await UpdateCartAsync(cart);
            }
            catch (Exception)
            {
                // Consider logging the exception
                return null;
            }
        }

        public async Task<Cart> UpdateItemQuantityAsync(string cartId, int productId, int quantity)
        {
            if (string.IsNullOrEmpty(cartId) || productId <= 0 || quantity < 0)
                return null;

            try
            {
                // Get existing cart
                var cart = await GetCartAsync(cartId);
                if (cart == null)
                    return null;

                // Find the item
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                    return cart; // Item not found, return unchanged cart

                if (quantity == 0)
                {
                    // If quantity is 0, remove the item
                    cart.Items.Remove(item);
                }
                else
                {
                    // Update the quantity
                    item.Quantity = quantity;
                }

                // Save the updated cart
                return await UpdateCartAsync(cart);
            }
            catch (Exception)
            {
                // Consider logging the exception
                return null;
            }
        }

    }
}
