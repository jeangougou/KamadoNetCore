using FluentAssertions;
using KamadoNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KamadoNetCoreTests
{
  public class UnitTests
  {
    #region Helpers
    private IWebHostBuilder GetNewHostBuilder(Action<Microsoft.AspNetCore.Builder.IApplicationBuilder> app)
    {
      return new WebHostBuilder()
        .UseKestrel()
        .UseIISIntegration()
        .Configure(app);
    }

    private async System.Threading.Tasks.Task<System.Net.Http.Headers.HttpResponseHeaders> GetHeaders(Action<Microsoft.AspNetCore.Builder.IApplicationBuilder> app)
    {
      var webHostBuilder = GetNewHostBuilder(app);
      using (var server = new Microsoft.AspNetCore.TestHost.TestServer(webHostBuilder))
      {
        var response = await server.CreateRequest("/").GetAsync();
        return response.Headers;
      }
    }

    private string RandomString(int length)
    {
      var random = new Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(System.Linq.Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<Boolean> ExpectHeadersFromMiddleware(
      Action<Microsoft.AspNetCore.Builder.IApplicationBuilder> appBuilder,
      string expectedHeaderKey,
      string expectedHeaderValue)
    {
      var headers = await GetHeaders(appBuilder);
      headers.Should().NotBeNullOrEmpty();
      headers.Contains(expectedHeaderKey).Should().BeTrue();
      headers.GetValues(expectedHeaderKey).Should().Contain(expectedHeaderValue);
      return true;
    }
    #endregion

    #region IeNoOpen - X-Download-Options
    [Fact]
    public async void TestUseIeNoOpen()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseIeNoOpen();
        },
        expectedHeaderKey: "X-Download-Options",
        expectedHeaderValue: "noopen"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region DnsPrefetchControl - X-DNS-Prefetch-Control
    [Fact]
    public async void TestEnableDnsPrefetchControl()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseDnsPrefetchControl(true);
        },
        expectedHeaderKey: "X-DNS-Prefetch-Control",
        expectedHeaderValue: "on"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestDisableDnsPrefetchControl()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseDnsPrefetchControl(false);
        },
        expectedHeaderKey: "X-DNS-Prefetch-Control",
        expectedHeaderValue: "off"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region XssFilter -  X-XSS-Protection
    [Fact]
    public async void TestDisableXssFilter()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseXssFilter(KamadoNetCore.Data.Headers.XssFilterToken.DisableXssFilter);
        },
        expectedHeaderKey: "X-XSS-Protection",
        expectedHeaderValue: "0"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestEnableSanitizedXssFilter()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseXssFilter(KamadoNetCore.Data.Headers.XssFilterToken.EnableSanitizedXssFilter);
        },
        expectedHeaderKey: "X-XSS-Protection",
        expectedHeaderValue: "1"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestEnableBlockedXssFilter()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseXssFilter(KamadoNetCore.Data.Headers.XssFilterToken.EnableBlockedXssFilter);
        },
        expectedHeaderKey: "X-XSS-Protection",
        expectedHeaderValue: "1; mode=block"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestEnableSanitizedAndReportXssFilter()
    {
      var ReportUri = RandomString(80);
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseXssFilter(KamadoNetCore.Data.Headers.XssFilterToken.EnableSanitizedAndReportXssFilter, ReportUri);
        },
        expectedHeaderKey: "X-XSS-Protection",
        expectedHeaderValue: "1; report={ReportUri}"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region NoSniff - X-Download-Options
    [Fact]
    public async void TestUseNoSniff()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseNoSniff();
        },
        expectedHeaderKey: "X-Content-Type-Options",
        expectedHeaderValue: "nosniff"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region ReferrerPolicy - Referrer-Policy
    [Fact(Skip = "Needs integration test")]
    public async void TestEmptyReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.None);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: ""
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestNoReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.NoReferrer);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "no-referrer"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestNoReferrerWhenDowngradePolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.NoReferrerWhenDowngrade);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "no-referrer-when-downgrade"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestOriginReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.Origin);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "origin"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestOriginWhenCrossOriginReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.OriginWhenCrossOrigin);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "origin-when-cross-origin"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestSameOriginReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.SameOrigin);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "same-origin"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestCustomReferrerPolicy()
    {
      var customReferrerPolicy = RandomString(80); 
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.SpecifyCustomString, customReferrerPolicy);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: customReferrerPolicy
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestStrictOriginReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.StrictOrigin);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "strict-origin"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestStrictOriginWhenCrossOriginReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.StrictOriginWhenCrossOrigin);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "strict-origin-when-cross-origin"
        ))
        .Should().BeTrue();
    }

    [Fact]
    public async void TestUnsafeUrlReferrerPolicy()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseReferrerPolicy(KamadoNetCore.Data.Headers.ReferrerPolicyToken.UnsafeUrl);
        },
        expectedHeaderKey: "Referrer-Policy",
        expectedHeaderValue: "unsafe-url"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region HidePoweredBy - X-Powered-By
    [Fact(Skip = "Needs integration test with kestrel to return default PoweredBy")]
    public async void TestHidePoweredBy()
    {
      var pretendToBe = RandomString(80);
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseHidePoweredBy(pretendToBe);
        },
        expectedHeaderKey: "X-Powered-By",
        expectedHeaderValue: "{pretendToBe}"
        ))
        .Should().BeTrue();
    }
    #endregion

    #region FrameGuard - X-Frame-Options

    [Fact]
    public async void TestDenyFrameGuard()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseFrameGuard(KamadoNetCore.Data.Headers.FrameGuardToken.Deny);
        },
        expectedHeaderKey: "X-Frame-Options",
        expectedHeaderValue: "DENY"
        )).Should().BeTrue();
    }

    [Fact]
    public async void TestSameOriginFrameGuard()
    {
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseFrameGuard(KamadoNetCore.Data.Headers.FrameGuardToken.SameOrigin);
        },
        expectedHeaderKey: "X-Frame-Options",
        expectedHeaderValue: "SAMEORIGIN"
        )).Should().BeTrue();
    }

    [Fact]
    public async void TestAllowFromFrameGuard()
    {
      var allowFrom = RandomString(80);
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseFrameGuard(KamadoNetCore.Data.Headers.FrameGuardToken.AllowFrom, allowFrom);
        },
        expectedHeaderKey: "X-Frame-Options",
        expectedHeaderValue: string.Format("ALLOW-FROM {0}", allowFrom)
        )).Should().BeTrue();
    }

    #endregion

    #region StrictTransportSecurity - Strict-Transport-Security
    [Fact]
    public async void TestSTS()
    {
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseStrictTransportSecurity(maxAge, false);
        },
        expectedHeaderKey: "Strict-Transport-Security",
        expectedHeaderValue: string.Format("max-age={0}", maxAge.TotalSeconds)
        )).Should().BeTrue();
    }
    [Fact]
    public async void TestSTSWithSubDomains()
    {
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UseStrictTransportSecurity(maxAge, true);
        },
        expectedHeaderKey: "Strict-Transport-Security",
        expectedHeaderValue: string.Format("max-age={0}; includeSubDomains", maxAge.TotalSeconds)
        )).Should().BeTrue();
    }
    #endregion

    #region PublicKeyPinning - Public-Key-Pins
    [Fact]
    public async void TestPKPNosubdomain()
    {
      var pins = new string[]
      {
        RandomString(23),
        RandomString(24)
      };
      var reportUri = RandomString(34);
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UsePublicKeyPinning(pins, maxAge, false, reportUri);
        },
        expectedHeaderKey: "Public-Key-Pins",
        expectedHeaderValue: string.Format("pin-sha256=\"{0}\"; pin-sha256=\"{1}\"; max-age={2}; report-uri=\"{3}\"", 
        pins[0], 
        pins[1],
        maxAge.TotalSeconds,
        reportUri)
        )).Should().BeTrue();
    }

    [Fact]
    public async void TestPKPAll()
    {
      var pins = new string[]
      {
        RandomString(23),
        RandomString(24)
      };
      var reportUri = RandomString(34);
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UsePublicKeyPinning(pins, maxAge, true, reportUri);
        },
        expectedHeaderKey: "Public-Key-Pins",
        expectedHeaderValue: string.Format("pin-sha256=\"{0}\"; pin-sha256=\"{1}\"; max-age={2}; includeSubDomains; report-uri=\"{3}\"",
        pins[0],
        pins[1],
        maxAge.TotalSeconds,
        reportUri)
        )).Should().BeTrue();
    }

    [Fact]
    public async void TestPKPWithSubdomains()
    {
      var pins = new string[]
      {
        RandomString(23),
        RandomString(24)
      };
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UsePublicKeyPinning(pins, maxAge, true);
        },
        expectedHeaderKey: "Public-Key-Pins",
        expectedHeaderValue: string.Format("pin-sha256=\"{0}\"; pin-sha256=\"{1}\"; max-age={2}; includeSubDomains",
        pins[0],
        pins[1],
        maxAge.TotalSeconds)
        )).Should().BeTrue();
    }

    [Fact]
    public async void TestPKPMandatoryOnly()
    {
      var pins = new string[]
      {
        RandomString(23),
        RandomString(24)
      };
      var rnd = new Random();
      var maxAge = new TimeSpan(
        days: rnd.Next(1, 356),
        hours: rnd.Next(1, 23),
        minutes: rnd.Next(1, 59),
        seconds: rnd.Next(1, 59));
      (await ExpectHeadersFromMiddleware(
        appBuilder: app =>
        {
          app.UsePublicKeyPinning(pins, maxAge);
        },
        expectedHeaderKey: "Public-Key-Pins",
        expectedHeaderValue: string.Format("pin-sha256=\"{0}\"; pin-sha256=\"{1}\"; max-age={2}",
        pins[0],
        pins[1],
        maxAge.TotalSeconds)
        )).Should().BeTrue();
    }
    #endregion
  }
}
