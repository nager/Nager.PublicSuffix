> [!IMPORTANT]
> The current stable development branch **V2** can be found [here](https://github.com/nager/Nager.PublicSuffix/tree/v2)<br>
> I am currently working on a new version. This includes some breaking changes.


Nager.PublicSuffix
==========
The TLD proliferation makes it difficult to check whether domain names are valid. This project uses the rules of publicsuffix.org, a list of known public domain suffixes (TLD) to validate and split domains into three the parts (TLD, domain, subdomain). The validation rules are loaded directly from https://publicsuffix.org. The List is maintained here [publicsuffix list - GitHub](https://github.com/publicsuffix/list)

For example, if you use Dmarc for e-mail security, it is very important to know what the main domain is.

A domain name has 3 major parts:

Example | Top Level Domain (TLD) | Domain | Subdomain |
--- | --- | --- | --- |
blog.google.com | com | google | blog |
www.wikipedia.org | org | wikipedia | www |
mail.yandex.ru | ru | yandex | mail |
www.amazon.co.uk | co.uk | amazon | www |

## nuget
The package is available on [nuget](https://www.nuget.org/packages/Nager.PublicSuffix)
```
PM> install-package Nager.PublicSuffix
```

## Features
- High performance
- FileTldRuleProvider or WebTldRuleProvider
- CacheProvider
- Async support

## Examples

### Analyze domain
Without a custom config the `WebTldRuleProvider` has a default cache live time of 1 day, then you must refresh the cache with execute `BuildAsync`;
```cs
var domainParser = new DomainParser(new WebTldRuleProvider());

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.Hostname = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.SubDomain = "sub";
//domainInfo.TLD = "co.uk";
```

### Check is a valid domain
Without a custom config the `WebTldRuleProvider` has a default cache live time of 1 day, then you must refresh the cache with execute `BuildAsync`;
```cs
var domainParser = new DomainParser(new WebTldRuleProvider());

var isValid = domainParser.IsValidDomain("sub.test.co.uk");
```

### Change the default cache time
```cs
//cache data for 10 hours
var cacheProvider = new FileCacheProvider(cacheTimeToLive: new TimeSpan(10, 0, 0));
var webTldRuleProvider = new WebTldRuleProvider(cacheProvider: cacheProvider);

var domainParser = new DomainParser(webTldRuleProvider);
for (var i = 0; i < 100; i++)
{
    var isValid = webTldRuleProvider.CacheProvider.IsCacheValid();
    if (!isValid)
    {
        webTldRuleProvider.BuildAsync().GetAwaiter().GetResult(); //Reload data
    }
	
    var domainInfo = domainParser.Parse($"sub{i}.test.co.uk");
}
```

### Use a local publicsuffix data file
```cs
var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.Hostname = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.SubDomain = "sub";
//domainInfo.TLD = "co.uk";
```
