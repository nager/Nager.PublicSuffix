Thank you for using the Nager.PublicSuffix package (https://github.com/tinohager/Nager.PublicSuffix)
----------------------------------------------------------------
Please support this project with the award of a GitHub Star (★)


Examples:
----------------------------------------------------------------

var domainParser = new DomainParser(new WebTldRuleProvider());

var domainName = domainParser.Get("sub.test.co.uk");
//domainName.Domain = "test";
//domainName.Hostname = "sub.test.co.uk";
//domainName.RegistrableDomain = "test.co.uk";
//domainName.SubDomain = "sub";
//domainName.TLD = "co.uk";
