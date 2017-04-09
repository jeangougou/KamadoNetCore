using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class IeNoOpenMiddleware
  {
    private readonly RequestDelegate _next;
    private const string HEADER_VALUE = "noopen";
    private const string HEADER_KEY = "X-Download-Options";

    public IeNoOpenMiddleware(RequestDelegate next)
    {
      _next = next;
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