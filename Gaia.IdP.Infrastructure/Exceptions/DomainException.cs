using System;
using Gaia.IdP.Infrastructure.Enums;

namespace Gaia.IdP.Infrastructure.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(ErrorStatusCode statusCode, ErrorMessage errorMessage, string message)
        : base(message) { this.ErrorStatusCode = statusCode; this.ErrorMessage = errorMessage; }

        public DomainException(ErrorStatusCode statusCode, ErrorMessage errorMessage)
        : base(null) { this.ErrorStatusCode = statusCode; this.ErrorMessage = errorMessage; }

        public DomainException(ErrorStatusCode statusCode, ErrorMessage errorMessage, Exception innerException)
        : base(null, innerException) { this.ErrorStatusCode = statusCode; this.ErrorMessage = errorMessage; }

        public DomainException(ErrorStatusCode statusCode, ErrorMessage errorMessage, string message, Exception innerException)
        : base(message, innerException) { this.ErrorStatusCode = statusCode; this.ErrorMessage = errorMessage; }

        public DomainException(ErrorStatusCode statusCode, string message)
        : base(message) { this.ErrorStatusCode = statusCode; }

        public DomainException(ErrorStatusCode statusCode)
        : base(null) { this.ErrorStatusCode = statusCode; }

        public DomainException(ErrorStatusCode statusCode, Exception innerException)
        : base(null, innerException) { this.ErrorStatusCode = statusCode; }

        public DomainException(ErrorStatusCode statusCode, string message, Exception innerException)
        : base(message, innerException) { this.ErrorStatusCode = statusCode; }
        
        public DomainException(string message) : base(message)
        {
        }

        public ErrorStatusCode ErrorStatusCode { get; set; }

        public ErrorMessage? ErrorMessage { get; set; }
    }
}