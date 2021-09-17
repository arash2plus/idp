using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.Infrastructure.Results
{
    public class DomainServerErrorResult : DomainResultBase
    {
        public DomainServerErrorResult(Exception exception, HttpContext context)
        {
            Init(exception);

            if (exception is NotImplementedException)
                Status = (int)HttpStatusCode.NotImplemented;
                
            else
                Status = (int)HttpStatusCode.InternalServerError;

            Exception = exception.Message;
            TraceId = Activity.Current?.Id ?? context?.TraceIdentifier;
        }

        private void Init(Exception exception)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6";
            Title = "serverErrorOccurred";
        }
        
        public string Type { get; private set; }
        public string Title { get; private set; }
        public string Exception { get; private set; }
    }
}
