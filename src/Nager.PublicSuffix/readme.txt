
████████ ██   ██  █████  ███    ██ ██   ██     ██    ██  ██████  ██    ██
   ██    ██   ██ ██   ██ ████   ██ ██  ██       ██  ██  ██    ██ ██    ██
   ██    ███████ ███████ ██ ██  ██ █████         ████   ██    ██ ██    ██
   ██    ██   ██ ██   ██ ██  ██ ██ ██  ██         ██    ██    ██ ██    ██
   ██    ██   ██ ██   ██ ██   ████ ██   ██        ██     ██████   ██████


Thank you for using this project. This project is completely free for commercial use.

However, if you use our project commercially we would like you to support us with a sponsorship.
The maintenance and support costs time and we would like to ensure this for the future with your help.
You can easily support us via the Github Sponsor function. https://github.com/sponsors/nager

We would also be very happy about a GitHub Star ★

Project Source: https://github.com/nager/Nager.PublicSuffix



Examples of use:

Get DomainInfo for sub.test.co.uk
══════════════════════════════════════════════════════════════════════════════════════════════════════

    var domainParser = new DomainParser(new WebTldRuleProvider());
    
    var domainInfo = domainParser.Parse("sub.test.co.uk");
    //domainInfo.Domain = "test";
    //domainInfo.Hostname = "sub.test.co.uk";
    //domainInfo.RegistrableDomain = "test.co.uk";
    //domainInfo.SubDomain = "sub";
    //domainInfo.TLD = "co.uk";

Check is sub.test.co.uk a valid domain
══════════════════════════════════════════════════════════════════════════════════════════════════════

var isValid = domainParser.IsValidDomain("sub.test.co.uk");
