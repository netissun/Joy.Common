using Enyim.Caching;
using Enyim.Caching.Memcached;
using System;

namespace Joy.Common.Caching
{
    public static class MemCachedTool
    {
        static MemcachedClient client = null;

        static MemCachedTool()
        {
            client = new MemcachedClient("enyim.com/memcached");
        }
        public static bool Add(string key, object value, int maxTimeExpired)
        {
            TimeSpan validFor = new TimeSpan(0, 0, maxTimeExpired);
            return client.Store(StoreMode.Add, key, value, validFor);
        }
        public static bool Add(string key, object value)
        {
            return client.Store(StoreMode.Add, key, value);
        }
        public static bool Set(string key, object value, int maxTimeExpired)
        {
            TimeSpan validFor = new TimeSpan(0, 0, maxTimeExpired);
            return client.Store(StoreMode.Set, key, value, validFor);
        }
        public static bool Set(string key, object value)
        {
            return client.Store(StoreMode.Set, key, value);
        }
        public static T Get<T>(string key) where T : class
        {
            return client.Get<T>(key);
        }
        public static T Pop<T>(string key) where T : class
        {
            T obj = client.Get<T>(key);
            if (obj == null) return obj;
            client.Remove(key);
            return obj;
        }
        public static bool Remove(string key)
        {
            return client.Remove(key);
        }
        public static void RemoveAll()
        {
            client.FlushAll();
        }

    }
}
