using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public interface ICacheProvider
    {
        Task<string> GetAsync();
        Task SetAsync(string data);
        bool IsCacheValid();
    }
}
