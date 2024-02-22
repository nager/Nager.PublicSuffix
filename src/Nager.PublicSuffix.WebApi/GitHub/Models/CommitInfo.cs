namespace Nager.PublicSuffix.WebApi.GitHub.Models
{
    public class CommitInfo
    {
        public Author? Author { get; set; }
        public Committer? Committer { get; set; }
        public string? Message { get; set; }
        public Tree? Tree { get; set; }
        public string? Url { get; set; }
        public Verification? Verification { get; set; }
    }
}
