using Microsoft.Extensions.Caching.Distributed;
using OnlineLearningPlatform.Domain.Abstract;
using System.Text.Json;

namespace OnlineLearningPlatform.Infrastruture.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _redis;

        public CacheService(IDistributedCache redis)
        {
            _redis = redis;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var cachedData = await _redis.GetStringAsync(key, ct);
            if (string.IsNullOrEmpty(cachedData)) return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                // Nếu không truyền thời gian hết hạn, mặc định để 30 phút
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(30)
            };

            var jsonData = JsonSerializer.Serialize(value);
            await _redis.SetStringAsync(key, jsonData, options, ct);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            // 1. Xóa trên Redis (Luồng 2 của CR7 trong ảnh)
            await _redis.RemoveAsync(key, ct);
        }
    }
}
