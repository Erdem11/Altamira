using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altamira.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Altamira.RedisCache
{
    public class RedisCache : ICache
    {
        private readonly string _connection;

        public RedisCache(string connection)
        {
            _connection = connection;
        }

        public async Task DeleteCache<T>(T obj, int id) where T : new()
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var key = $"{obj.GetType().Name}_{id}";
            var db = cm.GetDatabase(0);
            await db.KeyDeleteAsync(key);
        }

        public async Task DeleteCache(string key)
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var db = cm.GetDatabase(0);
            await db.KeyDeleteAsync(key);
        }

        public async Task SetCache<T>(T obj, int id, TimeSpan expire = default) where T : new()
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var key = $"{obj.GetType().Name}_{id}";
            var db = cm.GetDatabase(0);

            var json = JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            await db.StringSetAsync(key, json, expire);
        }

        public async Task SetCache<T>(T obj, string key, TimeSpan expire = default) where T : new()
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var db = cm.GetDatabase(0);

            var json = JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            await db.StringSetAsync(key, json, expire);
        }

        public async Task<T> GetCache<T>(int id) where T : new()
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var key = $"{typeof(T).Name}_{id}";
            var db = cm.GetDatabase(0);

            var json = await db.StringGetAsync(key);
            if (json == default)
            {
                return default;
            }
            var value = JsonConvert.DeserializeObject<T>(json);

            return value;
        }

        public async Task<T> GetCache<T>(string key) where T : new()
        {
            IConnectionMultiplexer cm = await ConnectionMultiplexer.ConnectAsync(_connection);

            var db = cm.GetDatabase(0);

            var json = await db.StringGetAsync(key);
            if (json == default)
            {
                return default;
            }
            var value = JsonConvert.DeserializeObject<T>(json);

            return value;
        }
    }
}