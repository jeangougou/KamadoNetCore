using System.ComponentModel;

namespace KamadoNetCore.Data.Headers
{
  public enum ReferrerPolicyToken
  {
    [Description("No policy set")]
    None = 0,
    [Description("Specify a custom referrer policy through the optional attribute")]
    SpecifyCustomString = 1,
    [Description("")]
    NoReferrer = 10,
    [Description("")]
    NoReferrerWhenDowngrade = 20,
    [Description("")]
    StrictOrigin = 30,
    [Description("")]
    StrictOriginWhenCrossOrigin = 40,
    [Description("")]
    SameOrigin = 50,
    [Description("")]
    Origin = 60,
    [Description("")]
    OriginWhenCrossOrigin = 70,
    [Description("")]
    UnsafeUrl = 80
  };
}
