using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class PublicKeyPinning
  {
    private readonly RequestDelegate _next;
    private readonly string HEADER_VALUE;
    private const string HEADER_KEY = "Public-Key-Pins";
    private readonly double _maxAge;
    private readonly bool _includeSubDomains;
    private readonly string[] _pinSha256;
    private readonly string _reportUri;

    public PublicKeyPinning(RequestDelegate next,
      string[] PinSha256,
      TimeSpan maxAge,
      bool includeSubDomains = false,
      string reportUri = "")
    {
      _next = next;
      _maxAge = maxAge.TotalSeconds;
      _includeSubDomains = includeSubDomains;
      _pinSha256 = PinSha256;
      _reportUri = reportUri;

      HEADER_VALUE = string.Format("max-age={0}{1}",
        _maxAge,
        (includeSubDomains) ? "; includeSubDomains" : string.Empty
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
              .Add(HEADER_KEY, ParseHeaderValue(_pinSha256, _maxAge, _includeSubDomains, _reportUri));
        return Task.CompletedTask;
      });
      await _next(context);
    }

    private string ParseHeaderValue(
      string[] PinSha256,
      double maxAge,
      bool includeSubDomains = false,
      string reportUri = "")
    {
      var parsedReportUri = string.Empty;
      if (!string.IsNullOrEmpty(reportUri)) {
        parsedReportUri = string.Format("; report-uri=\"{0}\"", reportUri);
      }
      var postFix = string.Format("max-age={0}{1}{2}",
        _maxAge,
        (includeSubDomains) ? "; includeSubDomains" : string.Empty,
        parsedReportUri
        );

      var prefix = PinSha256.Select(p => string.Format("pin-sha256=\"{0}\"", p)).Aggregate((current, next) =>
      {
        return string.Format("{0}; {1}", current, next);
      });
      return string.Format("{0}; {1}", prefix, postFix);
    }
  }
}