using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nager.PublicSuffix.Website.Model;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicSuffixController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Task<DomainReport>> Get(string domain)
        {
            var domainParser = new DomainParser(new WebTldRuleProvider());
            var domainName = domainParser.Get(domain);
            var valid = domainParser.IsValidDomain(domain);

            var domainReport = new DomainReport
            {
                Valid = valid,
                TLD = domainName.TLD,
                Domain = domainName.Domain,
                SubDomain = domainName.SubDomain,
                RegistrableDomain = domainName.RegistrableDomain,
                Hostname = domainName.Hostname,
            };

            return StatusCode(StatusCodes.Status200OK, domainReport);
        }
    }
}
