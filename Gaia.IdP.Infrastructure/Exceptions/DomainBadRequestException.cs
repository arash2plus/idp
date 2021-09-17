using System;
using Gaia.IdP.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gaia.IdP.Infrastructure.Exceptions
{
    public class DomainBadRequestException : Exception
    {
        public DomainBadRequestException(string key, ErrorMessage errorMessage)
        : base() { this.Key = key; this.ErrorMessage = errorMessage; }

        public DomainBadRequestException(string key, string errorMessage)
        : base() { this.Key = key; this.ErrorMessageStr = errorMessage; }

        public DomainBadRequestException(ActionExecutingContext context)
        : base() { this.ActionExecutingContext = context; }

        
        public string Key { get; set; }
        public ErrorMessage? ErrorMessage { get; set; }
        public string ErrorMessageStr { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
    }
}