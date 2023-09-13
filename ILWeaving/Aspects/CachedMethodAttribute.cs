using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;

namespace AOPDemo.ILWeaving.Aspects
{
    [PSerializable]
    public sealed class CachedMethodAttribute : OnMethodBoundaryAspect
    {
        [NonSerialized]
        private static readonly MemoryCache Cache;

        // This field will be set by CompileTimeInitialize and serialized at build time, then deserialized at runtime.
        private string methodName;

        static CachedMethodAttribute()
        {
            if (!PostSharpEnvironment.IsPostSharpRunning)
            {
                Cache = MemoryCache.Default;
            }
        }

        // Method executed at build time.
        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            this.methodName = method.DeclaringType.FullName + "." + method.Name;
        }

        // This method is executed before the execution of target methods of this aspect.
        public override void OnEntry(MethodExecutionArgs args)
        {
            // Compute the cache key.
            string cacheKey = GetCacheKey(args.Instance, args.Arguments);

            // Fetch the value from the cache.
            object value = Cache.Get(cacheKey);

            if (value != null)
            {
                // The value was found in cache. Don't execute the method. Return immediately.
                args.ReturnValue = value;
                args.FlowBehavior = FlowBehavior.Return;
            }
            else
            {
                // The value was NOT found in cache. Continue with method execution, but store
                // the cache key so that we don't have to compute it in OnSuccess.
                args.MethodExecutionTag = cacheKey;
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            string cacheKey = (string)args.MethodExecutionTag;

            Cache.Add(cacheKey, args.ReturnValue, DateTimeOffset.Now.AddSeconds(30));
        }

        private string GetCacheKey(object instance, Arguments arguments)
        {
            // If we have no argument, return just the method name so we don't uselessly allocate memory.
            if (instance == null && arguments.Count == 0)
                return this.methodName;

            // Add all arguments to the cache key. Note that generic arguments are not part of the cache
            // key, so method calls that differ only by generic arguments will have conflicting cache keys.
            StringBuilder sb = new StringBuilder(this.methodName);
            sb.Append('(');

            if (instance != null)
            {
                sb.Append(instance);
                sb.Append("; ");
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                sb.Append(arguments.GetArgument(i) ?? "null");
                if (i != arguments.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
