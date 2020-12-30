Thank you for using the Nager.PublicSuffix package (https://github.com/nager/Nager.PublicSuffix)
----------------------------------------------------------------
Please support this project with the award of a GitHub Star (★)


Examples:
----------------------------------------------------------------

var domainParser = new DomainParser(new WebTldRuleProvider());

var domainInfo = domainParser.Parse("sub.test.co.uk");
//domainInfo.Domain = "test";
//domainInfo.Hostname = "sub.test.co.uk";
//domainInfo.RegistrableDomain = "test.co.uk";
//domainInfo.SubDomain = "sub";
//domainInfo.TLD = "co.uk";


var isValid = domainParser.IsValidDomain("sub.test.co.uk");