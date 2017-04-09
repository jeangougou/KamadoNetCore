using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class StrictTransportSecurityMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly string HEADER_VALUE;
    private const string HEADER_KEY = "Strict-Transport-Security";
    private readonly double _maxAge;
    private readonly bool _includeSubDomains;

    public StrictTransportSecurityMiddleware(
      RequestDelegate next, 
      TimeSpan maxAge, 
      bool includeSubDomains = false)
    {
      _next = next;
      _maxAge = maxAge.TotalSeconds;
      _includeSubDomains = includeSubDomains;

      HEADER_VALUE = string.Format("max-age={0}{1}", 
        _maxAge,
        (includeSubDomains) ? "; includeSubDomains": string.Empty
        );
    }

    public async Task Invoke(HttpContext context)
    {
      context.Response.OnStarting(() =>
      {
        context.Response.Headers.Remove(HEADER_KEY);
        context
              .Response
              .Headers
              .Add(HEADER_KEY, HEADER_VALUE);
        return Task.CompletedTask;
      });
      await _next(context);
    }
  }
}