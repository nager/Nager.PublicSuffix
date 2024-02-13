using System;

namespace Nager.PublicSuffix.Exceptions
{
    /// <summary>
    /// Rule Load Exception
    /// </summary>
    public class RuleLoadException : Exception
    {
        /// <summary>
        /// Rule Load Exception
        /// </summary>
        /// <param name="errorMessage"></param>
        public RuleLoadException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
