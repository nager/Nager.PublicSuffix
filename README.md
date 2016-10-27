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
	var publicSuffix = new PublicSuffix();
	var data = await publicSuffix.LoadAsync();
	var tldRules = publicSuffix.Parse(data);
	publicSuffix.Add(tldRules);

	var domainName = publicSuffix.Get("sub.test.co.uk");
```