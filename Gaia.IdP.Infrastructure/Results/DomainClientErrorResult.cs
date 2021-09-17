using System.Diagnostics;
using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.Infrastructure.Results
{
    public class DomainClientErrorResult : DomainResultBase
    {
        public DomainClientErrorResult(DomainException exception, HttpContext context)
        {
            Init();

            var error = exception.ErrorMessage.HasValue ?
                exception.ErrorMessage.ToString() :
                exception.Message;

            Status = (int)exception.ErrorStatusCode;
            Error = error;
            MoreDetails = exception.Message.Contains("DomainException") ? null : exception.Message;
            TraceId = Activity.Current?.Id ?? context?.TraceIdentifier;
        }

        private void Init()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5";
            Title = "clientErrorOccurred";
        }
        
        public string Type { get; private set; }
        public string Title { get; private set; }
        public string Error { get; }
        public string MoreDetails { get; }
    }
}
