using System;

namespace Nager.PublicSuffix.Exceptions {
    public class NonResolvedDomainNameException : Exception {
        public NonResolvedDomainNameException (string message) : base (message) { }
    }
}