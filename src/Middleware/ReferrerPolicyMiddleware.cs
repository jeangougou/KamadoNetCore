using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KamadoNetCore.Middleware
{
  /// <summary>
  /// Set referrer policy at header level
  /// </summary>
  public class ReferrerPolicyMiddleware
  {
    // @ref:https://www.w3.org/TR/referrer-policy/#referrer-policy-header
    private const string HEADER_KEY = "Referrer-Policy";
    private readonly RequestDelegate _next;
    private readonly Data.Headers.ReferrerPolicyToken _policy;
    private readonly string _customReferrerPolicy;

    public ReferrerPolicyMiddleware(RequestDelegate next, Data.Headers.ReferrerPolicyToken policy, string CustomReferrerPolicy = "")
    {
      _next = next;
      if (Data.Headers.ReferrerPolicyToken.SpecifyCustomString == policy)
      {
        _customReferrerPolicy = CustomReferrerPolicy;
      }
      _policy = policy;
    }

    public async Task Invoke(HttpContext context)
    {
      context.Response.OnStarting(() =>
      {
        context.Response.Headers.Remove(HEADER_KEY);
        if (Data.Headers.ReferrerPolicyToken.None == _policy)
        {
          return Task.CompletedTask;
        }
        context
              .Response
              .Headers
              .Add(HEADER_KEY, GetHeaderValue(policy: _policy, CustomReferrerPolicy: _customReferrerPolicy));
        return Task.CompletedTask;
      });
      await _next(context);
    }

    internal Microsoft.Extensions.Primitives.StringValues GetHeaderValue(Data.Headers.ReferrerPolicyToken policy, string CustomReferrerPolicy = "")
    {
      switch (policy)
      {
        case Data.Headers.ReferrerPolicyToken.SpecifyCustomString:
          return new[] { CustomReferrerPolicy };
        case Data.Headers.ReferrerPolicyToken.NoReferrer:
          return new[] { "no-referrer" };
        case Data.Headers.ReferrerPolicyToken.NoReferrerWhenDowngrade:
          return new[] { "no-referrer-when-downgrade" };
        case Data.Headers.ReferrerPolicyToken.StrictOrigin:
          return new[] { "strict-origin" };
        case Data.Headers.ReferrerPolicyToken.StrictOriginWhenCrossOrigin:
          return new[] { "strict-origin-when-cross-origin" };
        case Data.Headers.ReferrerPolicyToken.SameOrigin:
          return new[] { "same-origin" };
        case Data.Headers.ReferrerPolicyToken.Origin:
          return new[] { "origin" };
        case Data.Headers.ReferrerPolicyToken.OriginWhenCrossOrigin:
          return new[] { "origin-when-cross-origin" };
        case Data.Headers.ReferrerPolicyToken.UnsafeUrl:
          return new[] { "unsafe-url" };
        case Data.Headers.ReferrerPolicyToken.None:
        default:
          return new[] { "" };
      }
    }
  }
}
