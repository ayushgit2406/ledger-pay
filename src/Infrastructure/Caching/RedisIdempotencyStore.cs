using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Caching
{
    public class RedisIdempotencyStore : IIdempotencyStore
    {
        private readonly IDatabase _db;
        public RedisIdempotencyStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public Task<bool> ExistsAsync(string key, CancellationToken ct)
        {
            return _db.KeyExistsAsync(key);
        }

        public Task StoreAsync(string key, CancellationToken ct)
        {
            return _db.StringSetAsync(key, "1", TimeSpan.FromMinutes(10));
        }
    }
}