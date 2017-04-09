using KamadoNetCore.Data.Headers;
using Microsoft.AspNetCore.Builder;
using System;

namespace KamadoNetCore
{
  public static class MiddlewareExtension
  {
    public static IApplicationBuilder UseReferrerPolicy(this IApplicationBuilder builder, ReferrerPolicyToken policy, string CustomReferrerPolicy = "")
    {
      return builder.UseMiddleware<Middleware.ReferrerPolicyMiddleware>(policy, CustomReferrerPolicy);
    }

    public static IApplicationBuilder UseDnsPrefetchControl(this IApplicationBuilder builder, bool enableDnsPrefetch = false)
    {
      return builder.UseMiddleware<Middleware.DnsPrefetchControlMiddleware>(enableDnsPrefetch);
    }

    public static IApplicationBuilder UseNoSniff(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<Middleware.NoSniffMiddleware>();
    }

    public static IApplicationBuilder UseIeNoOpen(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<Middleware.IeNoOpenMiddleware>();
    }

    public static IApplicationBuilder UseXssFilter(this IApplicationBuilder builder, XssFilterToken xssFilter, string ReportUri = "")
    {
      return builder.UseMiddleware<Middleware.XssFilterMiddleware>(xssFilter, ReportUri);
    }

    public static IApplicationBuilder UseHidePoweredBy(this IApplicationBuilder builder, string PretendToBe = "")
    {
      return builder.UseMiddleware<Middleware.HidePoweredByMiddleware>(PretendToBe);
    }

    public static IApplicationBuilder UseFrameGuard(this IApplicationBuilder builder, FrameGuardToken frameGuardToken, string AllowFrom = "")
    {
      return builder.UseMiddleware<Middleware.FrameGuardMiddleware>(frameGuardToken, AllowFrom);
    }

    public static IApplicationBuilder UseStrictTransportSecurity(this IApplicationBuilder builder, 
      TimeSpan MaxAge, 
      bool IncludeSubDomains = false)
    {
      return builder.UseMiddleware<Middleware.StrictTransportSecurityMiddleware>(MaxAge, IncludeSubDomains);
    }

    public static IApplicationBuilder UsePublicKeyPinning(this IApplicationBuilder builder,
      string[] PinSha256,
      TimeSpan MaxAge,
      bool IncludeSubDomains = false,
      string ReportUri = "")
    {
      return builder.UseMiddleware<Middleware.PublicKeyPinning>(PinSha256, MaxAge, IncludeSubDomains, ReportUri);
    }
  }
}
