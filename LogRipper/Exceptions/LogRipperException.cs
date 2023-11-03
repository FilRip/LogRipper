using System;
using System.Runtime.Serialization;

namespace LogRipper.Exceptions
{
    [Serializable()]
    public class LogRipperException : Exception
    {
        public LogRipperException(string message) : base(message) { }

        protected LogRipperException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
