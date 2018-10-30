namespace Nager.PublicSuffix.Website.Model
{
    public class DomainReport
    {
        public bool Valid { get; set; }
        public string Domain { get; set; }
        public string TLD { get; set; }
        public string SubDomain { get; set; }
        public string RegistrableDomain { get; set; }
        public string Hostname { get; set; }
    }
}
