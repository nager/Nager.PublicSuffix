using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;

Console.WriteLine("Run - DemoLocalFileCachedHttpRuleProviderAsync");
await DemoLocalFileCachedHttpRuleProviderAsync();
Console.WriteLine("Run - DemoMemoryCachedHttpRuleProviderAsync");
await DemoMemoryCachedHttpRuleProviderAsync();
Console.WriteLine("Run - DemoSimpleHttpRuleProviderAsync");
await DemoSimpleHttpRuleProviderAsync();
Console.WriteLine("Run - DemoLocalFileRuleProviderAsync");
await DemoLocalFileRuleProviderAsync();

async Task DemoLocalFileCachedHttpRuleProviderAsync()
{
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    var ruleProviderLogger = loggerFactory.CreateLogger<CachedHttpRuleProvider>();

    IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new("Nager:PublicSuffix:DataUrl", "https://publicsuffix.org/list/public_suffix_list.dat")
        })
        .Build();

    using var httpClient = new HttpClient();
    var cacheProvider = new Nager.PublicSuffix.RuleProviders.CacheProviders.LocalFileSystemCacheProvider();

    var ruleProvider = new CachedHttpRuleProvider(ruleProviderLogger, configuration, cacheProvider, httpClient);

    await CheckAsync(ruleProvider);
}

async Task DemoMemoryCachedHttpRuleProviderAsync()
{
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    var ruleProviderLogger = loggerFactory.CreateLogger<CachedHttpRuleProvider>();

    IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new("Nager:PublicSuffix:DataUrl", "https://publicsuffix.org/list/public_suffix_list.dat")
        })
        .Build();

    using var httpClient = new HttpClient();
    var cacheProvider = new Nager.PublicSuffix.RuleProviders.CacheProviders.MemoryCacheProvider();

    var ruleProvider = new CachedHttpRuleProvider(ruleProviderLogger, configuration, cacheProvider, httpClient);

    await CheckAsync(ruleProvider);
}

async Task DemoSimpleHttpRuleProviderAsync()
{
    using var ruleProvider = new SimpleHttpRuleProvider();

    await CheckAsync(ruleProvider);
}

async Task DemoLocalFileRuleProviderAsync()
{
    var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");

    await CheckAsync(ruleProvider);
}

async Task CheckAsync(IRuleProvider ruleProvider)
{
    if (!await ruleProvider.BuildAsync(ignoreCache: true))
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Build error");
        Console.ResetColor();
        return;
    }

    var domainParser = new DomainParser(ruleProvider);
    var domainInfo = domainParser.Parse("www.google.com");

    if (domainInfo != null)
    {
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine($"{"TLD:",20} {domainInfo.TopLevelDomain}");
        Console.WriteLine($"{"FQDN:",20} {domainInfo.FullyQualifiedDomainName}");
        Console.WriteLine($"{"RegistrableDomain:",20} {domainInfo.RegistrableDomain}");
        Console.WriteLine($"{"Subdomain:",20} {domainInfo.Subdomain}");
        Console.WriteLine("------------------------------------------------");
    }

    var validDomains = new[] { "www.google.com", "amazon.com", "microsoft.de", "mail.google.com", "microsoft.de." };
    var invalidDomains = new[] { "www", "uk", "co.uk", ".", "test@test.com" };

    foreach (var validDomain in validDomains)
    {
        Console.WriteLine($"{validDomain} -> {domainParser.IsValidDomain(validDomain)}");
    }

    foreach (var invalidDomain in invalidDomains)
    {
        Console.WriteLine($"{invalidDomain} -> {domainParser.IsValidDomain(invalidDomain)}");
    }
}