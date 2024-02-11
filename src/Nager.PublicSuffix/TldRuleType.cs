namespace Nager.PublicSuffix
{
    /// <summary>
    /// TLD Rule type, defined by www.publicsuffix.org
    /// </summary>
    public enum TldRuleType
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Wildcard
        /// </summary>
        Wildcard,

        /// <summary>
        /// WildcardException
        /// </summary>
        WildcardException,
    }
}
