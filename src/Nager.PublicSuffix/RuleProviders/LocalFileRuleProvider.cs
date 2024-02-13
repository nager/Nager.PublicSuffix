using Nager.PublicSuffix.Extensions;
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
    public class LocalFileRuleProvider : IRuleProvider
    {
        private readonly string _filePath;
        private DomainDataStructure? _domainDataStructure;

        /// <summary>
        /// LocalFile RuleProvider
        /// </summary>
        /// <param name="filePath"></param>
        public LocalFileRuleProvider(string filePath)
        {
            this._filePath = filePath;
        }

        /// <inheritdoc/>
        public async Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default)
        {
            var ruleData = await this.LoadFromFile().ConfigureAwait(false);

            var ruleParser = new TldRuleParser();
            var rules = ruleParser.ParseRules(ruleData);

            var domainDataStructure = new DomainDataStructure("*", new TldRule("*"));
            domainDataStructure.AddRules(rules);

            this._domainDataStructure = domainDataStructure;

            return true;
        }

        /// <inheritdoc/>
        public DomainDataStructure? GetDomainDataStructure()
        {
            return this._domainDataStructure;
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
