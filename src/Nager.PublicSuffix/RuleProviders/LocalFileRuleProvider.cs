﻿using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleParsers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// LocalFile RuleProvider
    /// </summary>
    public class LocalFileRuleProvider : BaseRuleProvider
    {
        private readonly string _filePath;
        private readonly TldRuleDivisionFilter _tldRuleDivisionFilter;

        /// <summary>
        /// LocalFile RuleProvider
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tldRuleDivisionFilter"></param>
        public LocalFileRuleProvider(
            string filePath,
            TldRuleDivisionFilter tldRuleDivisionFilter = TldRuleDivisionFilter.All)
        {
            this._filePath = filePath;
            this._tldRuleDivisionFilter = tldRuleDivisionFilter;
        }

        /// <inheritdoc/>
        public override async Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default)
        {
            var ruleData = await this.LoadFromFile().ConfigureAwait(false);

            var ruleParser = new TldRuleParser(this._tldRuleDivisionFilter);
            var rules = ruleParser.ParseRules(ruleData);

            var domainDataStructure = new DomainDataStructure("*", new TldRule("*"));
            domainDataStructure.AddRules(rules);

            base.CreateDomainDataStructure(rules);

            return true;
        }

        private async Task<string> LoadFromFile()
        {
            if (!File.Exists(this._filePath))
            {
                throw new FileNotFoundException($"Rule file does not exist {this._filePath}");
            }

            using var reader = File.OpenText(this._filePath);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
