// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using Nager.PublicSuffix.RuleProviders.CacheProviders;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger("Program");
var ruleProviderLogger = loggerFactory.CreateLogger<CachedHttpRuleProvider>();

IConfiguration configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
    {
        new("Nager:PublicSuffix:DataUrl", "https://publicsuffix.org/list/public_suffix_list1.dat")
    })
    .Build();

var httpClient = new HttpClient();
var cacheProvider = new LocalFileSystemCacheProvider();

var ruleProvider = new CachedHttpRuleProvider(ruleProviderLogger, configuration, cacheProvider, httpClient);
await ruleProvider.BuildAsync(ignoreCache: true);

//var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");

var domainParser = new DomainParser(ruleProvider);
var domainInfo = domainParser.Parse("www.google.com");

if (domainInfo != null)
{
    logger.LogInformation($"TLD:{domainInfo.TopLevelDomain}");
    logger.LogInformation($"FQDN:{domainInfo.FullyQualifiedDomainName}");
    logger.LogInformation($"RegistrableDomain:{domainInfo.RegistrableDomain}");
}