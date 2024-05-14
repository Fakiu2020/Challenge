using System;

namespace NetChallenge.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() : base()
        {
        }

        public DuplicateEntityException(string message, Exception InnerException) : base(message,InnerException)
        {
        }
        public DuplicateEntityException(string message) : base(message)
        {
        }

      
    }
}
