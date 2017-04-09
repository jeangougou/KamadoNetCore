using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  internal class HidePoweredByMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly string _pretendToBe;
    private const string HEADER_KEY = "X-Powered-By";

    public HidePoweredByMiddleware(RequestDelegate next, string PretendToBe = "")
    {
      _next = next;
      _pretendToBe = PretendToBe;
    }
    public async Task Invoke(HttpContext context)
    {
      context.Response.OnStarting(() =>
      {
        context.Response.Headers.Remove(HEADER_KEY);
        if (!string.IsNullOrWhiteSpace(_pretendToBe))
        {
          context
                .Response
                .Headers
                .Add(HEADER_KEY, new[] { _pretendToBe });
        }
        return Task.CompletedTask;
      });
      await _next(context);
    }
  }
}