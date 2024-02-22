using Nager.PublicSuffix.WebApi.GitHub.Models;

namespace Nager.PublicSuffix.WebApi.GitHub
{
    /// <summary>
    /// GitHub Client
    /// </summary>
    public class GitHubClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// GitHub Client
        /// </summary>
        /// <param name="httpClient"></param>
        public GitHubClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._httpClient.DefaultRequestHeaders.Add("User-Agent", "Nager.PublicSuffix");
        }

        /// <summary>
        /// GetCommitAsync
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repository"></param>
        /// <param name="branch"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GitHubCommit?> GetCommitAsync(
            string owner,
            string repository,
            string branch,
            CancellationToken cancellationToken = default)
        {
            var requestUri = $"https://api.github.com/repos/{owner}/{repository}/commits/{branch}";

            return await this._httpClient.GetFromJsonAsync<GitHubCommit>(requestUri, cancellationToken);
        }
    }
}
