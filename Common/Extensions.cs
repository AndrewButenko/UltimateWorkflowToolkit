using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace UltimateWorkflowToolkit.Common
{
    public static class Extensions
    {
        public static string UrlEncode(this Dictionary<string, object> parameters)
        {
            return string.Join("&", parameters.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value.ToString())}"));
        }
    }
}
