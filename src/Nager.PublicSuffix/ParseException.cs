using System;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Parse Exception
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Reason of exception
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Parse Exception
        /// </summary>
        /// <param name="reason"></param>
        public ParseException(string reason)
        {
            this.Reason = reason;
        }
    }
}
