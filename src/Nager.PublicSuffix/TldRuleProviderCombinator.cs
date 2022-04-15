using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{

    /// <summary>
    /// Constructs a new TilRuleProvider by combining providerA and providerB
    /// </summary>
    public class TldRuleProviderCombinator : ITldRuleProvider
    {
        private readonly ITldRuleProvider _providerA;
        private readonly ITldRuleProvider _providerB;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="providerA"></param>
        /// <param name="providerB"></param>
        public TldRuleProviderCombinator(ITldRuleProvider providerA, ITldRuleProvider providerB)
        {
            _providerA = providerA;
            _providerB = providerB;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var rulesA = await _providerA.BuildAsync();
            var rulesB = await _providerB.BuildAsync();

            if (rulesA == null) return rulesB;
            if (rulesB == null) return rulesA;
            return Combine(rulesA, rulesB);
        }

        private static IEnumerable<TldRule> Combine(IEnumerable<TldRule> rulesA, IEnumerable<TldRule> rulesB)
        {
            var combined = 
                    rulesA
                        .Concat(rulesB)
                        .OrderBy(x=>x, new TldRuleComparer())
                        .DistinctBy(x=>x.Name)
                ;
            return combined;

        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var set = new HashSet<TKey>();

            foreach (var e in source)
            {
                if (set.Add(keySelector(e)))
                {
                    yield return e;
                }
            }
        }
    }
    internal class TldRuleComparer : IComparer<TldRule>
    {
        public int Compare(TldRule x, TldRule y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            
            // https://github.com/publicsuffix/list/wiki/Format
            var xReversed = string.Join(".", x.Name.Split('.').Reverse());
            var yReversed = string.Join(".", y.Name.Split('.').Reverse());
            return string.CompareOrdinal(xReversed, yReversed);
        }
    }
}