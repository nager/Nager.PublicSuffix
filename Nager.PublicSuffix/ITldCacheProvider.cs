using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public interface ITldCacheProvider
    {
        Task<string> GetValueAsync();
        Task SetValueAsync(string val);
        bool IsCacheValid();
    }
}