using System.ComponentModel;

namespace KamadoNetCore.Data.Headers
{
  public enum FrameGuardToken
  {
    [Description("Prevent page from hosting an iframe")]
    Deny = 10,
    [Description("Prevent all but same origin iframes")]
    SameOrigin = 20,
    [Description("Single whitelisted iframe origin")]
    AllowFrom = 30
  }
}
