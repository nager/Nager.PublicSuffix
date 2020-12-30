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
        private readonly DomainDataStructure _domainDataStructure;

        public PublicSuffixController(DomainDataStructure domainDataStructure)
        {
            this._domainDataStructure = domainDataStructure;
        }

        [HttpGet]
        public ActionResult<Task<DomainReport>> Get(string domain)
        {
            var domainParser = new DomainParser(this._domainDataStructure);
            var domainName = domainParser.Parse(domain);
            var valid = domainParser.IsValidDomain(domain);

            var domainReport = new DomainReport
            {
                Valid = valid,
                TLD = domainName?.TLD,
                Domain = domainName?.Domain,
                SubDomain = domainName?.SubDomain,
                RegistrableDomain = domainName?.RegistrableDomain,
                Hostname = domainName?.Hostname,
            };

            return StatusCode(StatusCodes.Status200OK, domainReport);
        }
    }
}
