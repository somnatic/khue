using System;

namespace mail_thomaslinder_at.Logic.Nodes
{
    public class KHueException : Exception
    {
        public KHueException()
        {
        }

        public KHueException(string message)
            : base(message)
        {
        }

        public KHueException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
