using Nager.PublicSuffix.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix.Extensions
{
    /// <summary>
    /// <see cref="DomainDataStructure"/> extension methods to create the TLD Tree.
    /// </summary>
    public static class DomainDataStructureExtensions
    {
        /// <summary>
        /// Add all the rules in <paramref name="tldRules"/> to <paramref name="structure"/>.
        /// </summary>
        /// <param name="structure">The structure to appened the rule.</param>
        /// <param name="tldRules">The rules to append.</param>
        public static void AddRules(this DomainDataStructure structure, IEnumerable<TldRule> tldRules)
        {
            foreach (var tldRule in tldRules)
            {
                structure.AddRule(tldRule);
            }
        }

        /// <summary>
        /// Add <paramref name="tldRule"/> to <paramref name="structure"/>.
        /// </summary>
        /// <param name="structure">The structure to append the rule.</param>
        /// <param name="tldRule">The rule to append.</param>
        public static void AddRule(this DomainDataStructure structure, TldRule tldRule)
        {
            var parts = tldRule.Name.Split('.').Reverse().ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                var domainPart = parts[i];
                if (parts.Count - 1 > i)
                {
                    //Check if domain exists
                    if (!structure.Nested.ContainsKey(domainPart))
                    {
                        structure.Nested.Add(domainPart, new DomainDataStructure(domainPart));
                    }

                    structure = structure.Nested[domainPart];
                    continue;
                }

                //Check if domain exists
                if (structure.Nested.ContainsKey(domainPart))
                {
                    structure.Nested[domainPart].TldRule = tldRule;
                    continue;
                }

                structure.Nested.Add(domainPart, new DomainDataStructure(domainPart, tldRule));
            }
        }

        /// <summary>
        /// Get collection of <see cref="TldRule"/> rules from <paramref name="structure"/>.
        /// </summary>
        /// <param name="structure">The <see cref="DomainDataStructure"/> structure from which to obtain the rules</param>
        /// <returns></returns>
        public static IEnumerable<TldRule> GetRules(this DomainDataStructure structure)
        {
            foreach (var dataStructure in structure.Nested.Values)
            {
                if (dataStructure.TldRule != null)
                {
                    yield return dataStructure.TldRule;
                }

                foreach (var tldRule in dataStructure.GetRules())
                {
                    yield return tldRule;
                }
            }
        }
    }
}
