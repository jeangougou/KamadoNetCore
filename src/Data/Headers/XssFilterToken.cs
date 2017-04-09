using System.ComponentModel;

namespace KamadoNetCore.Data.Headers
{
  public enum XssFilterToken
  {
    [Description("Disable xss filter")]
    DisableXssFilter = 10,
    [Description("Enable sanitized xss filter")]
    EnableSanitizedXssFilter = 20,
    [Description("Enable blocked xss filter")]
    EnableBlockedXssFilter,
    [Description("Enable sanitized xss filter and report")]
    EnableSanitizedAndReportXssFilter
  }
}
