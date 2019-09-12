using System;

namespace Nager.PublicSuffix.Exceptions {
    public class NonResolvedUrlException : Exception {
        public NonResolvedUrlException (string message) : base (message) { }
    }
}