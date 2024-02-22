namespace Nager.PublicSuffix.WebApi.GitHub.Models
{
    public class File
    {
        public string? Sha { get; set; }
        public string? Filename { get; set; }
        public string? Status { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int Changes { get; set; }
    }
}
