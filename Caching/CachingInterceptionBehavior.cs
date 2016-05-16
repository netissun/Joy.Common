using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Joy.Common.Object;

namespace Joy.Common.Caching
{
    public class CachingInterceptionBehavior : IInterceptionBehavior
    {
        private static IDictionary<string, MemoryCache> Cache = new Dictionary<string, MemoryCache>();
        private static IList<string> SyncLock = new List<string>();
        private static object SyncRoot = new object();

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        private object GetFromCache(Type declaringType, string key)
        {
            return Cache[declaringType.FullName].Get(key);
        }
        private bool IsInCache(Type declaringType, string key)
        {
            return Cache.ContainsKey(declaringType.FullName) && Cache[declaringType.FullName].Contains(key);
        }
        private void AddToCache(Type declaringType, string key, object toBeAddedToCache,int cacheTime)
        {
            if (!Cache.ContainsKey(declaringType.FullName))
            {
                Cache.Add(declaringType.FullName, new MemoryCache(declaringType.FullName));
            }

            CacheItem item = new CacheItem(key, toBeAddedToCache);
            Cache[declaringType.FullName].Add(item, new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(cacheTime)) });
        }
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            CacheAttribute attr = input.MethodBase.GetCustomAttribute<CacheAttribute>();
            if (attr==null)  return getNext()(input, getNext);
            var arguments = input.Arguments.ToJson();
            string key = string.Concat(input.MethodBase.DeclaringType.FullName, "|", input.MethodBase.ToString(), "|", arguments);
            if (IsInCache(input.MethodBase.DeclaringType, key))
            {
                return input.CreateMethodReturn(GetFromCache(input.MethodBase.DeclaringType, key));
            }
            lock (SyncRoot)
            {
                if (!SyncLock.Contains(key))
                {
                    SyncLock.Add(key);
                }
            }

            lock (SyncLock[SyncLock.IndexOf(key)])
            {
                if (IsInCache(input.MethodBase.DeclaringType, key))
                {
                    return input.CreateMethodReturn(GetFromCache(input.MethodBase.DeclaringType, key));
                }

                IMethodReturn result = getNext()(input, getNext);
                if (result.ReturnValue != null)
                {
                    AddToCache(input.MethodBase.DeclaringType, key, result.ReturnValue, attr.CacheTime);
                }
                return result;
            }
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
