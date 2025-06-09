using App.Application.Contracts.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace App.Caching;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : IRedisCacheService
{
    private readonly IDatabase _db = connectionMultiplexer.GetDatabase(1);
    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (!await _db.KeyExistsAsync(key))
            {
                return default;
            }

            var redisValue = await _db.StringGetAsync(key);

            if (redisValue.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(redisValue!);
        }
        catch (RedisConnectionException ex)
        {
            throw new Exception("Redis connection error", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to deserialize Redis value", ex);
        }
    }


    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        try
        {
            var serializedValue = JsonSerializer.Serialize(value);

            if (expiration.HasValue)
            {
                await _db.StringSetAsync(key, serializedValue, expiration);
            }
            else
            {
                await _db.StringSetAsync(key, serializedValue);
            }
        }
        catch (RedisConnectionException ex)
        {
            throw new Exception("Redis connection error", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to serialize value for Redis", ex);
        }
    }


    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        try
        {
            if (await _db.KeyExistsAsync(key))
            {
                await _db.KeyDeleteAsync(key);
            }
        }
        catch (RedisConnectionException ex)
        {
            throw new Exception("Redis connection error", ex);
        }
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        try
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            await _cacheLock.WaitAsync();
            try
            {
                cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var value = await factory();

                await SetAsync(key, value, expiration);

                return value;
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        catch (Exception ex)
        {
            if (factory != null)
            {
                return await factory();
            }
            throw new Exception("Cache operation failed and no fallback available", ex);
        }
    }
}