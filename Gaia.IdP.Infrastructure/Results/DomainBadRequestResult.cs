using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gaia.IdP.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gaia.IdP.Infrastructure.Results
{
    public class DomainBadRequestResult : DomainResultBase
    {
        public DomainBadRequestResult(DomainBadRequestException exception, HttpContext context)
        {
            Init();

            string errorMessage = exception.ErrorMessage.HasValue ?
                exception.ErrorMessage.ToString() :
                exception.ErrorMessageStr;

            TraceId = Activity.Current?.Id ?? context?.TraceIdentifier;
            Errors = new [] { new Error(exception.Key, errorMessage) };
        }

        public DomainBadRequestResult(string errorKey, string errorMessage, HttpContext context)
        {
            Init();

            TraceId = Activity.Current?.Id ?? context?.TraceIdentifier;
            Errors = new [] { new Error(errorKey, errorMessage) };
        }

        public DomainBadRequestResult(ActionExecutingContext context)
        {
            Init();
                        
            TraceId = Activity.Current?.Id ?? context.HttpContext?.TraceIdentifier;
            Errors = context.ModelState.SelectMany(m => m.Value.Errors.Select(o => new Error(m.Key, o.ErrorMessage)));
        }

        private void Init()
        {
            Status = 400;
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            Title = "validationFailed";
        }
        
        public string Type { get; private set; }
        public string Title { get; private set;}
        public IEnumerable<Error> Errors { get; }
    }

    public class Error
    {
        public string Key { get; set; }
        public IEnumerable<string> Values { get; set; }

        public Error(string key, string errorMessage)
        {
            Key = Char.ToLowerInvariant(key[0]) + key.Substring(1);

            Values = errorMessage?
                .Split(",")
                .Select(o => Char.ToLowerInvariant(o[0]) + o.Substring(1)) ??
                new string[0];

            if (Values.Any(o => o.Contains("is required")))
            {
                Values = Values.Select(o => {
                    if (o.Contains("is required"))
                        o = "isRequired";
                    
                    return o;
                });
            }
        }
    }
}
