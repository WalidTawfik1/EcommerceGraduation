using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IOptions<RedisSettings> redisSettings, ILogger<RedisService> logger)
        {
            _logger = logger;

            try
            {
                _logger.LogInformation($"Attempting to connect to Redis at {redisSettings.Value.Endpoint}");

                var configOptions = new ConfigurationOptions
                {
                    EndPoints = { redisSettings.Value.Endpoint },
                    User = redisSettings.Value.Username,
                    Password = redisSettings.Value.Password,
                    Ssl = true,
                    AbortOnConnectFail = false,
                    ConnectTimeout = 15000,
                    SyncTimeout = 15000,
                    ConnectRetry = 5
                };

                _redis = ConnectionMultiplexer.Connect(configOptions);

                _logger.LogInformation($"Redis connection established. Is connected: {_redis.IsConnected}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis connection failed: {ex.Message}");
                throw;
            }
        }

        public IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
    }

    public class RedisSettings
    {
        public string Endpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
