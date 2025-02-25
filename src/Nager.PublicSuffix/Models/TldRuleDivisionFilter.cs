namespace Nager.PublicSuffix.Models
{
    /// <summary>
    /// Specifies the type of TLD rules to include.
    /// </summary>
    public enum TldRuleDivisionFilter
    {
        /// <summary>
        /// Only officially recognized domains managed by ICANN.
        /// </summary>
        ICANNOnly,

        /// <summary>
        /// Only privately managed domains (e.g., blogspot.com, github.io).
        /// </summary>
        PrivateOnly,

        /// <summary>
        /// Both ICANN-managed and privately managed domains.
        /// </summary>
        All
    }
}
