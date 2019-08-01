using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Nager.PublicSuffix.Exceptions {
    public class InvalidUrlException : Exception {
        public InvalidUrlException (string message) : base (message) { }
    }
}