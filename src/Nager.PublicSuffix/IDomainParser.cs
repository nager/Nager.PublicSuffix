using System;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Interface Domain Parser
    /// </summary>
    public interface IDomainParser
    {
        /// <summary>
        /// Parse the DomainInfo from <paramref name="fullyQualifiedDomainName"/>.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo? Parse(string fullyQualifiedDomainName);

        /// <summary>
        /// Parse the DomainInfo from <paramref name="fullyQualifiedDomainName"/>.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <returns>DomainInfo object</returns>
        DomainInfo? Parse(Uri fullyQualifiedDomainName);

        /// <summary>
        /// Checks if the <paramref name="fullyQualifiedDomainName"/> is valid.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to check</param>
        /// <returns>Returns <strong>true</strong> if the <paramref name="fullyQualifiedDomainName"/> is valid.</returns>
        bool IsValidDomain(string fullyQualifiedDomainName);
    }
}