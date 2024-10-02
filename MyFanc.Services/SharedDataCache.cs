using Microsoft.Extensions.Caching.Memory;
using MyFanc.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Services
{
    public class SharedDataCache : ISharedDataCache
    {        
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheStorageDurations _cacheStorageDurations;
        private CancellationTokenSource _resetCacheToken = new();
        public SharedDataCache(IMemoryCache memoryCache, ICacheStorageDurations cacheStorageDurations)
        {
            _memoryCache = memoryCache;
            _cacheStorageDurations = cacheStorageDurations;
        }

        public T? GetData<T>(string key)
        {
            if(_memoryCache.TryGetValue(key, out T? data))
            {
                return data;
            }
            return default;
        }

        public void SetData<T>(string key, T data)
        {
            _memoryCache.Set(key, data, TimeSpan.FromHours(_cacheStorageDurations.CacheStorageDuration));
        }

        public void RemoveData<T>(string key)
        {
            _memoryCache.Remove(key);
        }

        public void ClearAllData()
        {
            var memoryCache = _memoryCache as MemoryCache;
            if (memoryCache != null)
            {
                memoryCache.Compact(1);
            }
        }
    }
}
