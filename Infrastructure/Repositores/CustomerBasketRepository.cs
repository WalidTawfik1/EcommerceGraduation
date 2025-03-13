using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using StackExchange.Redis;

namespace Ecom.Infrastructure.Repositores
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        private readonly IDatabase _database;
        public CustomerBasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public Task<bool> DeleteBasketAsync(string basketId)
        {
            return _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var result = await _database.StringGetAsync(basketId);
            if (!string.IsNullOrEmpty(result))
            {
                return JsonSerializer.Deserialize<CustomerBasket>(result);
            }
                return null;     
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var _basket = await _database.StringSetAsync(basket.Id.ToString(), JsonSerializer.Serialize(basket), TimeSpan.FromDays(3));
            if (_basket)
            {
                return await GetBasketAsync(basket.Id.ToString());
            }
            return null;
        }
    }
}
