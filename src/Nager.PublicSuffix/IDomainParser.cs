using System;
using System.Diagnostics.CodeAnalysis;

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
        /// <returns>A <see cref="DomainInfo"/> object containing details such as TopLevelDomain, Subdomain, RegistrableDomain, or null if the TopLevelDomain is invalid.</returns>
        DomainInfo? Parse(string fullyQualifiedDomainName);

        /// <summary>
        /// Parses a fully qualified domain name (FQDN) and retrieves information about the domain.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <returns>A <see cref="DomainInfo"/> object containing details such as TopLevelDomain, Subdomain, RegistrableDomain, or null if the TopLevelDomain is invalid.</returns>
        DomainInfo? Parse(Uri fullyQualifiedDomainName);

        /// <summary>
        /// Determines whether the specified <paramref name="fullyQualifiedDomainName"/> is valid.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to check</param>
        /// <returns><strong>True</strong> if the <paramref name="fullyQualifiedDomainName"/> is valid; otherwise, <strong>false</strong>.</returns>
        bool IsValidDomain(string fullyQualifiedDomainName);

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

        /// <summary>
        /// Attempts to parse a fully qualified domain name (FQDN) and extract its components.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name (FQDN) to parse</param>
        /// <param name="domainInfo">When successful, contains details about the domain, such as the TopLevelDomain, Subdomain, and RegistrableDomain; otherwise, null.</param>
        /// <returns>True if the FQDN was successfully parsed; false if the TopLevelDomain is invalid or parsing fails.</returns>
        bool TryParse(string fullyQualifiedDomainName, [NotNullWhen(true)] out DomainInfo? domainInfo);

#endif

    }
}