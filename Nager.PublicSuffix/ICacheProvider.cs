using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public interface ICacheProvider
    {
        Task<string> GetValueAsync();
        Task SetValueAsync(string val);
        bool IsCacheValid();
    }
}
