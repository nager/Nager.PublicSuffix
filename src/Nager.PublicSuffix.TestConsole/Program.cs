// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;

#region CachedHttpRuleProvider

//using var loggerFactory = LoggerFactory.Create(builder =>
//{
//    builder.AddConsole();
//});

//var ruleProviderLogger = loggerFactory.CreateLogger<CachedHttpRuleProvider>();

//IConfiguration configuration = new ConfigurationBuilder()
//    .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
//    {
//        new("Nager:PublicSuffix:DataUrl", "https://publicsuffix.org/list/public_suffix_list1.dat")
//    })
//    .Build();

//var httpClient = new HttpClient();
//var cacheProvider = new Nager.PublicSuffix.RuleProviders.CacheProviders.LocalFileSystemCacheProvider();

//var ruleProvider = new CachedHttpRuleProvider(ruleProviderLogger, configuration, cacheProvider, httpClient);

#endregion

#region

var ruleProvider = new SimpleHttpRuleProvider();

#endregion

#region LocalFileRuleProvider

//var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");

#endregion

await ruleProvider.BuildAsync(ignoreCache: true);

var domainParser = new DomainParser(ruleProvider);
var domainInfo = domainParser.Parse("www.google.com");

if (domainInfo != null)
{
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine($"{"TLD:", 20} {domainInfo.TopLevelDomain}");
    Console.WriteLine($"{"FQDN:", 20} { domainInfo.FullyQualifiedDomainName}");
    Console.WriteLine($"{"RegistrableDomain:", 20} {domainInfo.RegistrableDomain}");
    Console.WriteLine($"{"Subdomain:",20} {domainInfo.Subdomain}");
    Console.WriteLine("------------------------------------------------");
}


var validDomains = new[] { "www.google.com", "amazon.com", "microsoft.de", "mail.google.com" };
var invalidDomains = new[] { "www", "uk", "co.uk", ".", "test@test.com" };

foreach (var validDomain in validDomains)
{
    Console.WriteLine($"{validDomain} -> {domainParser.IsValidDomain(validDomain)}");
}

foreach (var invalidDomain in invalidDomains)
{
    Console.WriteLine($"{invalidDomain} -> {domainParser.IsValidDomain(invalidDomain)}");
}
