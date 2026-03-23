using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Library.Domain.Enrichers
{
    public class UserNameEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserNameEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", username));
        }
    }
}
