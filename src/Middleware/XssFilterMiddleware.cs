using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class XssFilterMiddleware
  {
    private const string HEADER_KEY = "X-XSS-Protection";
    private readonly RequestDelegate _next;
    private readonly Data.Headers.XssFilterToken _xssFilter;
    private readonly string _reportUri;

    public XssFilterMiddleware(RequestDelegate next, Data.Headers.XssFilterToken xssFilter, string ReportUri = "")
    {
      _next = next;
      if (Data.Headers.XssFilterToken.EnableSanitizedAndReportXssFilter == xssFilter)
      {
        _reportUri = ReportUri;
      }
      _xssFilter = xssFilter;
    }

    public async Task Invoke(HttpContext context)
    {
      context.Response.OnStarting(() =>
      {
        context.Response.Headers.Remove(HEADER_KEY);
        context
              .Response
              .Headers
              .Add(HEADER_KEY, GetHeaderValue(xssFilter: _xssFilter, ReportUri: _reportUri));

        return Task.CompletedTask;
      });
      await _next(context);
    }

    internal Microsoft.Extensions.Primitives.StringValues GetHeaderValue(Data.Headers.XssFilterToken xssFilter, string ReportUri = "")
    {
      switch (xssFilter)
      {
        case Data.Headers.XssFilterToken.DisableXssFilter:
          return new[] { "0" };
        case Data.Headers.XssFilterToken.EnableSanitizedXssFilter:
          return new[] { "1" };
        case Data.Headers.XssFilterToken.EnableBlockedXssFilter:
          return new[] { "1; mode=block" };
        case Data.Headers.XssFilterToken.EnableSanitizedAndReportXssFilter:
          return new[] { "1; report={ReportUri}" };
        default:
          return new[] { "" };
      }
    }
  }
}