using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders.CacheProviders
{
    /// <summary>
    /// ICacheProvider
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Get the data from the cache
        /// </summary>
        /// <returns>Returns null if the cache has expired</returns>
        Task<string?> GetAsync();

        /// <summary>
        /// Store data in the cache
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SetAsync(string data);

        /// <summary>
        /// Check if the cache is still valid
        /// </summary>
        /// <returns></returns>
        bool IsCacheValid();
    }
}
