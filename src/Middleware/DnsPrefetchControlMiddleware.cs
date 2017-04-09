using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class DnsPrefetchControlMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly bool _enableDnsPrefetch;
    private readonly string HEADER_VALUE;
    private const string HEADER_KEY = "X-DNS-Prefetch-Control";

    public DnsPrefetchControlMiddleware(RequestDelegate next, bool enableDnsPrefetch = false)
    {
      _next = next;
      _enableDnsPrefetch = enableDnsPrefetch;
      HEADER_VALUE = (enableDnsPrefetch) ? "on" : "off";
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