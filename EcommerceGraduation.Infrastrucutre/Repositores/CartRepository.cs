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
        public CartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();

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
    }
}
