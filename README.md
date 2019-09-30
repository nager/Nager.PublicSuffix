Nager.PublicSuffix
==========
The TLD proliferation makes it difficult to check whether domain names are valid. This project uses the rules of publicsuffix.org, a list of known public domain suffixes (TLD) to validate and split domains into three the parts (TLD, domain, subdomain). The validation rules are loaded directly from https://publicsuffix.org.

A domain name has 3 major parts:

TLD | Domain | Subdomain |
--- | --- | --- |
com | google | blog |
org | wikipedia | www |
ru | yandex | mail |

### Website
https://publicsuffix.nager.at/

### nuget
The package is available on [nuget](https://www.nuget.org/packages/Nager.PublicSuffix)
```
PM> install-package Nager.PublicSuffix
```

### Benefits
- Very fast
- Integrated cache
- Async

### Donation possibilities
If this project helps you reduce development time, you may buy me a beer :beer:
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/nagerat/25)

### Examples

#### Loading data from web (publicsuffix.org)
```cs
var domainParser = new DomainParser(new WebTldRuleProvider());

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```

#### Loading data from web change cache config
```cs
var webTldRuleProvider = new WebTldRuleProvider(cacheProvider: new FileCacheProvider(cacheTimeToLive: new TimeSpan(10, 0, 0))); //cache data for 10 hours

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

#### Loading data from file
```cs
var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```
