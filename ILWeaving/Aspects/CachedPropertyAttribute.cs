using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System.Runtime.Caching;

namespace AOPDemo.ILWeaving.Aspects
{
    /// <summary>
    /// This attribute turns a property/field into a Cache backed field with no real local storage.
    /// It is currently restricted to only be used on static properties and fields.
    /// It currently uses MemoryCache/ObjectCache but could easily be changed to use other providers.
    /// </summary>
    /// <example>
    /// <code>
    /// [CacheBacked(CacheKey = "YearList_Cached", CacheExpirationTime = 20)]
    /// private static VehicleYearList YearListCached { get; set; }
    /// </code>
    /// </example>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [LinesOfCodeAvoided(20)]
    public sealed class CachedPropertyAttribute : LocationInterceptionAspect
    {
        [NonSerialized]
        private static readonly MemoryCache Cache;

        private bool IsReadOnlyProperty;

        /// <summary>
        /// The unique key name for caching this item.
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// The amount of time to keep this object before expiration, in minutes.
        /// </summary>
        public double CacheExpirationTime { get; set; }

        public override void CompileTimeInitialize(LocationInfo locationInfo, AspectInfo aspectInfo)
        {
            if (locationInfo.LocationKind == LocationKind.Property)
            {
                IsReadOnlyProperty = !locationInfo.PropertyInfo.CanWrite;
            }
        }

        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {
            if (string.IsNullOrEmpty(CacheKey))
            {
                throw new InvalidAnnotationException("You must supply a CacheKey when using this attribute.");
            }

            if (CacheExpirationTime == 0)
            {
                throw new InvalidAnnotationException("You must supply a CacheExpirationTime when using this attribute.");
            }

            return true;
        }

        static CachedPropertyAttribute()
        {
            if (!PostSharpEnvironment.IsPostSharpRunning)
            {
                Cache = MemoryCache.Default;
            }
        }

        public override void OnGetValue(LocationInterceptionArgs args)
        {
            var item = Cache.Get(CacheKey);

            if (item != null)
            {
                args.Value = item;
            }
            else
            {
                base.OnGetValue(args);

                if (IsReadOnlyProperty)
                {
                    //Since this is a read-only property, we will cache the results from the Get instead.
                    AddItemToCache(args.Value);
                }
            }
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            if (args.Value != null)
            {
                AddItemToCache(args.Value);
                args.Value = null; //This is set to null so we don't actually store anything back to the field/property and use the cache instead.
                args.ProceedSetValue();
            }
        }

        private void AddItemToCache(object value)
        {
            Cache.Add(CacheKey, value, DateTimeOffset.Now.AddMinutes(CacheExpirationTime));
        }
    }
}
