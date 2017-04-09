using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class FrameGuardMiddleware
  {
    private readonly RequestDelegate _next;
    private const string HEADER_KEY = "X-Frame-Options";
    private readonly string _allowFrom;
    private readonly Data.Headers.FrameGuardToken _frameGuardToken;

    public FrameGuardMiddleware(RequestDelegate next, Data.Headers.FrameGuardToken frameGuardToken, string allowFrom = "")
    {
      _next = next;
      if (Data.Headers.FrameGuardToken.AllowFrom == frameGuardToken)
      {
        _allowFrom = allowFrom;
      }
      _frameGuardToken = frameGuardToken;
    }
    public async Task Invoke(HttpContext context)
    {
      context.Response.OnStarting(() =>
      {
        context.Response.Headers.Remove(HEADER_KEY);
        context
              .Response
              .Headers
              .Add(HEADER_KEY, GetHeaderValue(frameGuardToken: _frameGuardToken, allowFrom: _allowFrom));
        return Task.CompletedTask;
      });
      await _next(context);
    }

    internal Microsoft.Extensions.Primitives.StringValues GetHeaderValue(
      Data.Headers.FrameGuardToken frameGuardToken, 
      string allowFrom = "")
    {
      switch (frameGuardToken)
      {
        case Data.Headers.FrameGuardToken.Deny:
          return new[] { "DENY" };
        case Data.Headers.FrameGuardToken.SameOrigin:
          return new[] { "SAMEORIGIN" };
        case Data.Headers.FrameGuardToken.AllowFrom:
          return new[] { string.Format("ALLOW-FROM {0}", allowFrom) };
        default:
          return new[] { "" };
      }
    }
  }
}