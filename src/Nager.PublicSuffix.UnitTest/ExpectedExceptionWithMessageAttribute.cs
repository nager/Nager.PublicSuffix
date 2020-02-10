using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Nager.PublicSuffix.UnitTest
{
    public class ExpectedExceptionWithMessageAttribute : ExpectedExceptionBaseAttribute
    {
        public Type ExceptionType { get; set; }

        public string ExpectedMessage { get; set; }

        public ExpectedExceptionWithMessageAttribute(Type exceptionType)
        {
            this.ExceptionType = exceptionType;
        }

        public ExpectedExceptionWithMessageAttribute(Type exceptionType, string expectedMessage)
        {
            this.ExceptionType = exceptionType;
            this.ExpectedMessage = expectedMessage;
        }

        protected override void Verify(Exception e)
        {
            if (e.GetType() != this.ExceptionType)
            {
                Assert.Fail($"ExpectedExceptionWithMessageAttribute failed. Expected exception type: {this.ExceptionType.FullName}. " +
                    $"Actual exception type: {e.GetType().FullName}. Exception message: {e.Message}");
            }

            var actualMessage = e.Message.Trim();
            if (this.ExpectedMessage != null)
            {
                Assert.AreEqual(this.ExpectedMessage, actualMessage);
            }

            Debug.WriteLine($"ExpectedExceptionWithMessageAttribute:{actualMessage}");
        }
    }
}
