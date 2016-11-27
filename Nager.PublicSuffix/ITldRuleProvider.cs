using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public interface ITldRuleProvider
    {
        Task<IEnumerable<TldRule>> BuildAsync();
    }
}
