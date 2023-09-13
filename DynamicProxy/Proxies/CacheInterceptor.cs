using Castle.DynamicProxy;
using System.Runtime.Caching;

namespace AOPDemo.DynamicProxy.Proxies
{
    [Serializable]
    public class CacheInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // Generate a cache key based on the method name and arguments
            var cacheKey = GenerateCacheKey(invocation);

            // Try to retrieve the result from the cache
            var cache = MemoryCache.Default;
            var cachedResult = cache.Get(cacheKey);

            if (cachedResult != null)
            {
                // If the result is found in the cache, return it
                invocation.ReturnValue = cachedResult;
            }
            else
            {
                // If not found, proceed with the method execution
                invocation.Proceed();

                // Cache the result for future use
                cache.Add(cacheKey, invocation.ReturnValue, DateTimeOffset.Now.AddSeconds(60));
            }
        }

        private static string GenerateCacheKey(IInvocation invocation)
        {
            // Generate a unique cache key based on method name and arguments
            var methodName = invocation.Method.Name;
            var args = string.Join("_", invocation.Arguments.Select(a => a.ToString()));

            return $"{methodName}_{args}";
        }
    }
}
