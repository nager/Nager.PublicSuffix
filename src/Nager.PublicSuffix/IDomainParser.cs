using System;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Domain Parser Interface
    /// </summary>
    public interface IDomainParser
    {
        /// <summary>
        /// Parses a fully qualified domain name (FQDN) and retrieves information about the domain.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <returns>A DomainInfo object containing details such as TopLevelDomain, Subdomain, RegistrableDomain, or null if the TopLevelDomain is invalid.</returns>
        DomainInfo? Parse(string fullyQualifiedDomainName);

        /// <summary>
        /// Parses a fully qualified domain name (FQDN) and retrieves information about the domain.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <returns>A DomainInfo object containing details such as TopLevelDomain, Subdomain, RegistrableDomain, or null if the TopLevelDomain is invalid.</returns>
        DomainInfo? Parse(Uri fullyQualifiedDomainName);

        /// <summary>
        /// Checks if the <paramref name="fullyQualifiedDomainName"/> is valid.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to check</param>
        /// <returns>Returns <strong>true</strong> if the <paramref name="fullyQualifiedDomainName"/> is valid.</returns>
        bool IsValidDomain(string fullyQualifiedDomainName);
    }
}