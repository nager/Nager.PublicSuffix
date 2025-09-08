# Nager.PublicSuffix (PSL)

With so many different endings for domain names, it's hard to know if they're valid or not. This project uses a list from **publicsuffix.org**, which keeps track of all the common endings like `.com` or `.org`. It checks domain names against this list to see if they're okay. Then, it splits the domain into three parts: the ending (like .com), the main part (like google), and any subparts (like www). You can find the list on GitHub under [publicsuffix list repository](https://github.com/publicsuffix/list).

## Features
- High performance
- CacheProvider
- Async support
- Hot reload of rule data

## Use cases

- Cookie restriction for browsers
- Domain highlighting in the URL bar of browsers
- DMARC E-Mail Security
- Certificate requests (ACME)
- Determining Valid Wildcard Certificates
- Two-factor authentication (FIDO)

## Parts of a Domain

| Fully Qualified Domain Name (FQDN) | Top Level Domain (TLD)  | Domain     | Subdomain |
|---------------------------- | ------------------------------ | ---------- | --------- |
| blog.google.com             | com                            | google     | blog      |
| 22.cn                       | cn                             | 22         |           |
| www.volkswagen.de           | de                             | volkswagen | www       |
| www.amazon.co.uk            | co.uk                          | amazon     | www       |
| www.wikipedia.org           | org                            | wikipedia  | www       |

## Installation
The package is available on [NuGet](https://www.nuget.org/packages/Nager.PublicSuffix)
```powershell
PM> install-package Nager.PublicSuffix
```
## Usage Examples


### Automatic rule provider

Automatically downloads the public suffix list:

```cs
var ruleProvider = new SimpleHttpRuleProvider();
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var domainInfo = domainParser.Parse("sub.test.co.uk");
```

### Analyze domain
```cs
var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.FullyQualifiedDomainName = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.Subdomain = "sub";
//domainInfo.TopLevelDomain = "co.uk";
```

### Check is a valid domain
```cs
var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var isValid = domainParser.IsValidDomain("sub.test.co.uk");
```

### asp.net Intergration
```cs
// after -> var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); //Required for CachedHttpRuleProvider
builder.Services.AddSingleton<ICacheProvider, LocalFileSystemCacheProvider>();
builder.Services.AddSingleton<IRuleProvider, CachedHttpRuleProvider>();
builder.Services.AddSingleton<IDomainParser, DomainParser>();

// after -> var app = builder.Build();
var ruleProvider = app.Services.GetService<IRuleProvider>();
if (ruleProvider != null)
{
    await ruleProvider.BuildAsync();
}

// minimal api
app.MapGet("/DomainInfo/{domain}", (string domain, IDomainParser domainParser) =>
{
    domain = HttpUtility.UrlEncode(domain);

    var domainInfo = domainParser.Parse(domain);
    return domainInfo;
})
.WithName("DomainInfo")
.WithOpenApi();
```

### Console App Integration
```cs
var httpClient = new HttpClient();
var cacheProvider = new Nager.PublicSuffix.RuleProviders.CacheProviders.LocalFileSystemCacheProvider();
var ruleProvider = new CachedHttpRuleProvider(cacheProvider, httpClient);

await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);
var domainInfo = domainParser.Parse("example.com");
```
