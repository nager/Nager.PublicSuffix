Nager.PublicSuffix
==========

#####nuget
The package is available on nuget
https://www.nuget.org/packages/Nager.PublicSuffix
```
install-package Nager.PublicSuffix
```


#####Example
```cs
	var domainParser = new DomainParser();
	var data = await domainParser.LoadDataAsync();
	var tldRules = domainParser.ParseRules(data);
	domainParser.AddRules(tldRules);

	var domainName = domainParser.Get("sub.test.co.uk");
	//domainName.Domain = "test";
	//domainName.Hostname = "sub.test.co.uk";
	//domainName.RegistrableDomain = "test.co.uk";
	//domainName.SubDomain = "sub";
	//domainName.TLD = "co.uk";
```