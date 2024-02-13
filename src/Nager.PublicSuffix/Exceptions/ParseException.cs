using System;

namespace Nager.PublicSuffix.Exceptions
{
    /// <summary>
    /// Parse Exception
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Parse Exception
        /// </summary>
        /// <param name="errorMessage"></param>
        public ParseException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
