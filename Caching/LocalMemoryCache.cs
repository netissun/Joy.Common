using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Joy.Common.Caching
{
    public class LocalMemoryCache:MemoryCache
    {
        public LocalMemoryCache() 
            : base("defaultMemoryCache") 
        { 
        }
        private string CreateKeyWithRegion(string key, string regionName=null)
        {
            return "region:" + (string.IsNullOrEmpty(regionName) ? "null_region" : regionName) + ";key=" + key;
        }
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            Set(item.Key, item.Value, policy, item.RegionName);
        }
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            Set(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration },regionName);
        }
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            base.Set(CreateKeyWithRegion(key, regionName), value, policy);
        }
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            CacheItem temporary = base.GetCacheItem(CreateKeyWithRegion(key, regionName));
            return new CacheItem(CreateKeyWithRegion(key,regionName), temporary.Value);
        }
        public override object Get(string key, string regionName = null)
        {
            return base.Get(CreateKeyWithRegion(key, regionName));
        }
        public override object Remove(string key, string regionName = null)
        {
            return base.Remove(CreateKeyWithRegion(key,regionName));
        }
        public override bool Contains(string key, string regionName = null)
        {
            return base.Contains(CreateKeyWithRegion(key, regionName));
        }
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return (base.DefaultCacheCapabilities | System.Runtime.Caching.DefaultCacheCapabilities.CacheRegions);
            }
        }

        

    }
}
