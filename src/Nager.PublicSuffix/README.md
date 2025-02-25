# Nager.PublicSuffix (PSL)

With so many different endings for domain names, it's hard to know if they're valid or not.
This project uses a list from **publicsuffix.org**, which keeps track of all the common endings like `.com` or `.org`.
It checks domain names against this list to see if they're okay.
Then, it splits the domain into three parts: the ending (like .com), the main part (like google), and any subparts (like www).
You can find the list on GitHub under [publicsuffix list repository](https://github.com/publicsuffix/list).

## Use cases

- Cookie restriction for browsers
- Domain highlighting in the URL bar of browsers
- DMARC E-Mail Security
- Certificate requests (ACME)
- Determining Valid Wildcard Certificates
- Two-factor authentication (FIDO)

## Code Examples

### Analyze a Domain Using a Local Public Suffix List

Use a local public suffix list file to analyze domains

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

### Analyze a Domain Using the Online Public Suffix List

Use a remote source to always work with the latest public suffix list

```cs
var ruleProvider = new SimpleHttpRuleProvider();
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.FullyQualifiedDomainName = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.Subdomain = "sub";
//domainInfo.TopLevelDomain = "co.uk";
```