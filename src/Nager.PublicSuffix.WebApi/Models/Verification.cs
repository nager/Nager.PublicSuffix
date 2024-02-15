namespace Nager.PublicSuffix.WebApi.Models
{
    public class Verification
    {
        public bool Verified { get; set; } = false;
        public string? Reason { get; set; }
        public string? Signature { get; set; }
        public string? Payload { get; set; }
    }
}
