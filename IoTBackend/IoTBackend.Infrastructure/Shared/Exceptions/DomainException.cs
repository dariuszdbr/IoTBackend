using System;

namespace IoTBackend.Infrastructure.Shared.Exceptions
{
    public class DomainException : Exception
    {
        public int StatusCode { get; }

        public DomainException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        
    }
}