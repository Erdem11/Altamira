using System;
using System.Threading.Tasks;

namespace Altamira.Cache
{
    public interface ICache
    {
        public Task SetCache<T>(T obj, int id, TimeSpan expire = default) where T : new();

        public Task SetCache<T>(T obj, string key, TimeSpan expire = default) where T : new();

        public Task<T> GetCache<T>(int id) where T : new();

        public Task<T> GetCache<T>(string key) where T : new();

        public Task DeleteCache<T>(T obj, int id) where T : new();

        public Task DeleteCache(string key);
    }
}