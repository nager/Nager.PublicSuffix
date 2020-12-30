using System;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Domain parser
    /// </summary>
    public interface IDomainParser
    {
        /// <summary>
        /// Parse the DomainInfo from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo Parse(string domain);

        /// <summary>
        /// Parse the DomainInfo from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo Parse(Uri domain);

        /// <summary>
        /// Checks if it is a valid domain <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to check.</param>
        /// <returns><strong>true</strong> if <paramref name="domain"/> it's valid.</returns>
        bool IsValidDomain(string domain);
    }
}