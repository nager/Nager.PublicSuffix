
████████ ██   ██  █████  ███    ██ ██   ██     ██    ██  ██████  ██    ██
   ██    ██   ██ ██   ██ ████   ██ ██  ██       ██  ██  ██    ██ ██    ██
   ██    ███████ ███████ ██ ██  ██ █████         ████   ██    ██ ██    ██
   ██    ██   ██ ██   ██ ██  ██ ██ ██  ██         ██    ██    ██ ██    ██
   ██    ██   ██ ██   ██ ██   ████ ██   ██        ██     ██████   ██████


Thank you for using this project. This project is completely free for commercial use.

If you use it commercially, we would greatly appreciate your support through a sponsorship. 
Maintaining and supporting this project takes time, and your contribution helps ensure its future. 
You can easily sponsor us via GitHub: https://github.com/sponsors/nager

We would also be very happy about a GitHub Star ★

Project Source: https://github.com/nager/Nager.PublicSuffix



Examples of use:

-------------------------------------------------------------------------------
1. AUTOMATIC RULE PROVIDER (HTTP)
-------------------------------------------------------------------------------

// Automatically downloads the public suffix list:

var ruleProvider = new SimpleHttpRuleProvider();
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var domainInfo = domainParser.Parse("sub.test.co.uk");


-------------------------------------------------------------------------------
2. LOCAL RULE PROVIDER
-------------------------------------------------------------------------------

// Use a local copy of the public suffix list:

var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.FullyQualifiedDomainName = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.Subdomain = "sub";
//domainInfo.TopLevelDomain = "co.uk";


-------------------------------------------------------------------------------
3. VALIDATE A DOMAIN
-------------------------------------------------------------------------------

// Check whether a domain is valid:

var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
await ruleProvider.BuildAsync();

var domainParser = new DomainParser(ruleProvider);

var isValid = domainParser.IsValidDomain("sub.test.co.uk");
