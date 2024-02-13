using System;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Interface Domain Parser
    /// </summary>
    public interface IDomainParser
    {
        /// <summary>
        /// Parse the DomainInfo from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo? Parse(string domain);

        /// <summary>
        /// Parse the DomainInfo from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo? Parse(Uri domain);

        /// <summary>
        /// Checks if the <paramref name="domain"/> is valid.
        /// </summary>
        /// <param name="domain">The domain to check.</param>
        /// <returns>Returns <strong>true</strong> if the <paramref name="domain"/> is valid.</returns>
        bool IsValidDomain(string domain);
    }
}