﻿namespace Nager.PublicSuffix.WebApi.Models
{

    public class GitHubCommit
    {
        public string Sha { get; set; }
        public CommitInfo Commit { get; set; }
        public File[] Files { get; set; }
    }
}
