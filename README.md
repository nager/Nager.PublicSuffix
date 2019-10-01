Nager.PublicSuffix
==========
The TLD proliferation makes it difficult to check whether domain names are valid. This project uses the rules of publicsuffix.org, a list of known public domain suffixes (TLD) to validate and split domains into three the parts (TLD, domain, subdomain). The validation rules are loaded directly from https://publicsuffix.org.

A domain name has 3 major parts:

Example | TLD | Domain | Subdomain |
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
## Online test tool
You can try the logic right here [publicsuffix test tool](https://publicsuffix.nager.at)

## Benefits
- High performance
- FileTldRuleProvider or WebTldRuleProvider
- CacheProvider
- Async support

## Examples

### Loading data from web (publicsuffix.org)
Without any config the `WebTldRuleProvider` have a default cache live time of 1 day then you must refresh the cache with execute  `BuildAsync`;
```cs
var domainParser = new DomainParser(new WebTldRuleProvider());

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```

### Loading data from web change cache config
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
	
    var domainInfo = domainParser.Get($"sub{i}.test.co.uk");
}
```

### Loading data from file
```cs
var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```
