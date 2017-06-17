Nager.PublicSuffix
==========
publicsuffix.org a list of known public domain suffixes (TLD). Some example .com, .co.uk, co.at. The current data loads from https://publicsuffix.org. This library does not nees a additional cache system is very fast on different domain names.

##### nuget
The package is available on nuget
https://www.nuget.org/packages/Nager.PublicSuffix
```
install-package Nager.PublicSuffix
```


##### Example load data from publicsuffix.org
```cs
var domainParser = new DomainParser(new WebTldRuleProvider());

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```

##### Example load data from file
```cs
var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
```
