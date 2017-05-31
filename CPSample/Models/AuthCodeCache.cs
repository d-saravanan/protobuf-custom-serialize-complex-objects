using System.Collections.Concurrent;

namespace CPSample.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class GenericStaticCache<T>
    {
        /// <summary>
        /// The 
        /// </summary>
        private static ConcurrentDictionary<string, T> _ = new ConcurrentDictionary<string, T>();

        /// <summary>
        /// Add2s the typed object to cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Add2Cache(string key, T value)
        {
            _.GetOrAdd(key, value);
        }

        /// <summary>
        /// Gets the typed object data from the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get(string key)
        {
            _.TryGetValue(key, out T result);
            return result;
        }
    }
}