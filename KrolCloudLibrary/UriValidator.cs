using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrolCloudLibrary
{
    public static class UriValidator
    {
        private static CultureInfo ci = new CultureInfo("en-US");

        public static bool IsValidHtml(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) || !IsAbsoluteUrl(url))
            {
                return false;
            }

            if (url.Contains("bleacherreport.com"))
            {
                return url.Contains("/articles");
            }
            return url.EndsWith(".html") || url.EndsWith(".htm");
        }

        public static bool IsValidXml(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }
            return url.EndsWith(".xml", true, ci);
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}
